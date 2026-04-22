using GraphQL;
using Heroes.Contracts;
using Heroes.Contracts.HeroCategories;
using Heroes.Contracts.Heroes;
using Heroes.Contracts.Stats;
using Heroes.GrainClients.HeroCategories;
using Heroes.GrainClients.Heroes;
using Heroes.GrainClients.Statistics;
using Heroes.Grains;
using Heroes.Server;
using Heroes.Server.Gql;
using Heroes.Server.Gql.Core;
using Heroes.Server.Infrastructure;
using Heroes.Server.Realtime;
using Heroes.Server.Sample;
using Heroes.Server.Tenancy;
using Scalar.AspNetCore;
using Serilog;
using Sketch7.Multitenancy;
using Sketch7.Multitenancy.AspNet;
using Sketch7.Multitenancy.Orleans;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var appInfo = new AppInfo(builder.Configuration);
Console.Title = $"{appInfo.Name} - {appInfo.Environment}";

// Logging
builder.Host.UseSerilog((ctx, loggerConfig) =>
{
	loggerConfig
		.Enrich.FromLogContext()
		.ReadFrom.Configuration(ctx.Configuration)
		.Enrich.WithMachineName()
		.Enrich.WithDemystifiedStackTraces()
		.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext:l}] {Message:lj}{NewLine}{Exception}");

	loggerConfig.WithAppInfo(appInfo);
});

// Core services
builder.Services.AddSingleton<IAppInfo>(appInfo);

var tenantRegistry = new AppTenantRegistry();
builder.Services
	.AddMultitenancy<AppTenant>(opts => opts
		.WithRegistry<IAppTenantRegistry>(tenantRegistry)
		.WithHttpResolver<AppTenant, AppTenantHttpResolver>()
		.WithServices(tsb => tsb
			.For("lol", s => s.AddSingleton<IHeroDataClient, MockLoLHeroDataClient>())
			.For("hots", s => s.AddSingleton<IHeroDataClient, MockHotsHeroDataClient>())
		)
	);

builder.Services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);

// Orleans
builder.Host.UseOrleans((ctx, silo) =>
{
	silo
		.AddMemoryStreams(OrleansConstants.STREAM_PROVIDER)
		.AddMemoryGrainStorage("PubSubStore")
		.UseAppConfiguration(new AppSiloBuilderContext
		{
			AppInfo = appInfo,
			HostBuilderContext = ctx,
			SiloOptions = new AppSiloOptions
			{
				SiloPort = GetAvailablePort(11111, 12000),
				GatewayPort = GetAvailablePort(30000, 31000),
			}
		})
		.AddIncomingGrainCallFilter<LoggingIncomingCallFilter>()
		.AddStartupTask<WarmupStartupTask>()
		.UseMultitenancy<AppTenant>()
		.UseSignalR(cfg =>
		{
			cfg.Configure((siloBuilder, signalrBuilderConfig) =>
				siloBuilder.UseStorage(signalrBuilderConfig.StorageProvider, appInfo, storeName: "SignalR"));
		});
});

// Web services
builder.Services
	.AddSingleton<IHeroService, HeroService>()
	.AddSingleton<IHeroStatsGrainClient, HeroStatsGrainClient>()
	.AddScoped<IHeroCategoryGrainClient, HeroCategoryGrainClient>()
	.AddScoped<IHeroGrainClient, HeroGrainClient>()
;
builder.Services.AddCustomAuthentication();
builder.Services.AddSignalR()
	.AddJsonProtocol(opts =>
	{
		opts.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		opts.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
	})
	.AddOrleans();

builder.Services.AddCors(o => o.AddPolicy("TempCorsPolicy", policy =>
{
	policy
		.SetIsOriginAllowed(_ => true)
		.AllowAnyMethod()
		.AllowAnyHeader()
		.AllowCredentials();
}));

builder.Services.AddGraphQL(gql => gql
	.AddSystemTextJson(options =>
	{
		options.AllowTrailingCommas = true;
		options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		options.ReadCommentHandling = JsonCommentHandling.Skip;
		options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
	})
	.AddDataLoader()
	// .AddExecutionStrategySelector<DefaultExecutionStrategySelector>()
	.AddSchema<AppSchema>()
	.AddGraphTypes()
	.AddUserContextBuilder(httpContext => new GraphQLUserContext
	{
		User = httpContext.User,
		// Capture grain clients from the HTTP request scope where the tenant accessor is already set
		// by the multitenancy middleware. Avoids the scope isolation issue with GraphQL.NET's
		// internal execution scope.
		HeroGrainClient = httpContext.RequestServices.GetRequiredService<IHeroGrainClient>(),
		HeroCategoryGrainClient = httpContext.RequestServices.GetRequiredService<IHeroCategoryGrainClient>(),
	})
);
builder.Services.AddOpenApi();

var app = builder.Build();

// Middleware pipeline
app.UseCors("TempCorsPolicy");

// Lightweight health probe — registered BEFORE multitenancy middleware so it
// always returns 200 regardless of the {tenant} route param.
app.MapGet("/ping", () => Results.Ok("pong")).ExcludeFromDescription();

app.UseGraphQLPlayground("/", new()
{
	GraphQLEndPoint = "/graphql",
	SubscriptionsEndPoint = "/graphql",
});

if (app.Environment.IsDevelopment())
	app.UseDeveloperExceptionPage();

app.UseRouting();
// Apply multitenancy middleware to /api/ routes and /graphql — SignalR hubs
// and other endpoints don't carry a tenant identifier.
app.UseWhen(
	ctx => ctx.Request.Path.StartsWithSegments("/api")
		  || ctx.Request.Path.StartsWithSegments("/graphql"),
	branch => branch.UseMultitenancy<AppTenant>()
);

app.UseAuthorization();

// Hubs
app.MapHub<HeroHub>("/real-time/hero");
app.MapHub<UserNotificationHub>("/userNotifications");

// Heroes REST endpoints
app.MapGet("/api/{tenant}/heroes", (string tenant, IHeroGrainClient client) => client.GetAll())
	.WithTags("Heroes");

app.MapGet("/api/{tenant}/heroes/{id}", (string tenant, string id, IHeroGrainClient client) => client.Get(id))
	.WithTags("Heroes");

// GraphQL endpoint — tenant resolved from the X-Tenant request header
app.MapGraphQL("/graphql");

// OpenAPI + Scalar
app.MapOpenApi();
app.MapScalarApiReference();

await app.RunAsync();

static int GetAvailablePort(int start, int end)
{
	for (var port = start; port < end; ++port)
	{
		var listener = TcpListener.Create(port);
		listener.ExclusiveAddressUse = true;
		try
		{
			listener.Start();
			return port;
		}
		catch (SocketException)
		{
		}
		finally
		{
			listener.Stop();
		}
	}

	throw new InvalidOperationException("No available port found in range.");
}

// Required for WebApplicationFactory<Program> from external test assemblies
public partial class Program { }
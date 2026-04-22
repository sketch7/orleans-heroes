using GraphQL;
using Heroes.Server.Gql;
using Microsoft.OpenApi;
using Orleans.Configuration;
using Scalar.AspNetCore;
using Serilog;
using Sketch7.Multitenancy.AspNet;
using Sketch7.Multitenancy.Orleans;
using System.Text.Json;
using System.Text.Json.Nodes;
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

var tenantRegistry = new AppTenantRegistry();

// Core services
builder.Services
	.AddSingleton<IAppInfo>(appInfo)
	.AddSingleton<IHeroStatsGrainClient, HeroStatsGrainClient>()
	.AddScoped<IHeroCategoryGrainClient, HeroCategoryGrainClient>()
	.AddScoped<IHeroGrainClient, HeroGrainClient>()
	.AddMultitenancy<AppTenant>(opts => opts
		.WithRegistry<IAppTenantRegistry>(tenantRegistry)
		.WithHttpResolver<AppTenant, AppTenantHttpResolver>()
		.WithServices(tsb => tsb
			.For("lol", s => s.AddSingleton<IHeroDataClient, MockLoLHeroDataClient>())
			.For("hots", s => s.AddSingleton<IHeroDataClient, MockHotsHeroDataClient>())
		)
	);

builder.Services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);

// Global JSON options for minimal API endpoints — camelCase properties + string enums
builder.Services.ConfigureHttpJsonOptions(opts =>
{
	opts.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
	opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

// Orleans
builder.Host.UseOrleans((_, silo) =>
{
	silo
		.AddMemoryStreams(OrleansConstants.StreamProvider)
		.AddMemoryGrainStorage(OrleansConstants.GrainMemoryStorage)
		.AddMemoryGrainStorage(OrleansConstants.GrainPersistenceStorage)
		.AddMemoryGrainStorage(OrleansConstants.PubSubStore)
		// TODO: Redis — add a compatible persistence package and replace AddMemoryGrainStorage calls above
		// .AddRedisGrainStorage(OrleansConstants.GrainPersistenceStorage, cfg => cfg.Configure(options => { ... }))
		.Configure<ClusterOptions>(options =>
		{
			options.ClusterId = appInfo.ClusterId;
			options.ServiceId = appInfo.Name;
		})
		// TODO: production clustering — replace UseLocalhostClustering with the appropriate provider
		// .UseDevelopmentClustering(options => options.PrimarySiloEndpoint = new IPEndPoint(IPAddress.Loopback, 11111))
		// .ConfigureEndpoints(IPAddress.Loopback, 11111, 30000)
		// .Configure<GrainCollectionOptions>(options => options.CollectionAge = TimeSpan.FromMinutes(1.5))
		// .Configure<ClusterMembershipOptions>(options => options.ExpectedClusterSize = 1)
		.UseLocalhostClustering()
		.AddIncomingGrainCallFilter<LoggingIncomingCallFilter>()
		.AddStartupTask<WarmupStartupTask>()
		.UseMultitenancy<AppTenant>()
		.UseSignalR(cfg =>
		{
			cfg.Configure((siloBuilder, signalrBuilderConfig) => siloBuilder
				.AddMemoryGrainStorage(signalrBuilderConfig.StorageProvider)
			);
		});
});

// Web services
builder.Services
	.AddAppAuth()
	.AddCors(o => o.AddPolicy("TempCorsPolicy", policy =>
	{
		policy
			.SetIsOriginAllowed(_ => true)
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials();
	}))
	.AddGraphQL(gql => gql
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
		.AddUserContextBuilder(httpContext => new AppGqlUserContext
		{
			User = httpContext.User,
			// Capture grain clients from the HTTP request scope where the tenant accessor is already set
			// by the multitenancy middleware. Avoids the scope isolation issue with GraphQL.NET's
			// internal execution scope.
			HeroGrainClient = httpContext.RequestServices.GetRequiredService<IHeroGrainClient>(),
			HeroCategoryGrainClient = httpContext.RequestServices.GetRequiredService<IHeroCategoryGrainClient>(),
		})
	)
	.AddOpenApi(opts =>
	{
		// Pre-fill the {tenant} path parameter in Scalar with a sensible default.
		opts.AddOperationTransformer((operation, _, _) =>
		{
			if (operation.Parameters is null)
				return Task.CompletedTask;

			foreach (var param in operation.Parameters.OfType<OpenApiParameter>().Where(p => p.Name == "tenant"))
			{
				param.Examples = new Dictionary<string, IOpenApiExample>
				{
					["lol"] = new OpenApiExample { Value = JsonNode.Parse("\"lol\"") },
					["hots"] = new OpenApiExample { Value = JsonNode.Parse("\"hots\"") },
				};
			}

			return Task.CompletedTask;
		});

		// Pre-fill the {id} parameter for the heroes-by-id endpoint.
		opts.AddOperationTransformer((operation, context, _) =>
		{
			if (operation.Parameters is null || context.Description.RelativePath?.Contains("heroes/{id}") != true)
				return Task.CompletedTask;

			foreach (var param in operation.Parameters.OfType<OpenApiParameter>().Where(p => p.Name == "id"))
			{
				param.Examples = new Dictionary<string, IOpenApiExample>
				{
					["default"] = new OpenApiExample { Value = JsonNode.Parse("\"rengar\"") }
				};
			}

			return Task.CompletedTask;
		});
	});

builder.Services.AddSignalR()
	.AddJsonProtocol(opts =>
	{
		opts.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		opts.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
	})
	.AddOrleans();


var app = builder.Build();

// Middleware pipeline
app.UseCors("TempCorsPolicy");

// Lightweight health probe — registered BEFORE multitenancy middleware so it
// always returns 200 regardless of the {tenant} route param.
app.MapGet("/ping", () => Results.Ok("pong")).ExcludeFromDescription();

app.UseGraphQLGraphiQL("/ui/graphql", new()
{
	GraphQLEndPoint = "/graphql",
	Headers = new()
	{
		["X-Tenant"] = "lol",
	},
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

// Required for WebApplicationFactory<Program> from external test assemblies
public partial class Program;

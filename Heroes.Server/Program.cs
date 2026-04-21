using Heroes.Contracts;
using Heroes.Contracts.Heroes;
using Heroes.GrainClients;
using Heroes.Server;
using Heroes.Server.Gql;
using Heroes.Server.Infrastructure;
using Heroes.Server.Realtime;
using Heroes.Server.Sample;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Scalar.AspNetCore;
using Serilog;
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
builder.Services.AddSingleton<IAppTenantRegistry, AppTenantRegistry>();
builder.Services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);

// Orleans
builder.Host.UseOrleans((ctx, silo) =>
{
	silo
		.ConfigureServices(services => services.AddAppGrains())
		.AddMemoryStreams(OrleansConstants.STREAM_PROVIDER)
		.AddMemoryGrainStorage("PubSubStore")
		.UseAppConfiguration(new AppSiloBuilderContext
		{
			AppInfo = appInfo,
			HostBuilderContext = ctx,
			SiloOptions = new AppSiloOptions
			{
				SiloPort = GetAvailablePort(11111, 12000),
				GatewayPort = 30001,
			}
		})
		.AddIncomingGrainCallFilter<LoggingIncomingCallFilter>()
		.AddStartupTask<WarmupStartupTask>()
		.UseSignalR(cfg =>
		{
			cfg.Configure((siloBuilder, signalrBuilderConfig) =>
				siloBuilder.UseStorage(signalrBuilderConfig.StorageProvider, appInfo, storeName: "SignalR"));
		});
});

// Web services
builder.Services.AddSingleton<IHeroService, HeroService>();
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

builder.Services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);

builder.Services.AddAppClients();
builder.Services.AddAppGraphQL();
builder.Services.AddOpenApi();

var app = builder.Build();

// Middleware pipeline
app.UseCors("TempCorsPolicy");

app.UseGraphQL("/graphql");
app.UseGraphQLPlayground("/", new()
{
	GraphQLEndPoint = "/graphql",
	SubscriptionsEndPoint = "/graphql",
});

if (app.Environment.IsDevelopment())
	app.UseDeveloperExceptionPage();

app.UseRouting();
app.UseAuthorization();

// Hubs
app.MapHub<HeroHub>("/real-time/hero");
app.MapHub<UserNotificationHub>("/userNotifications");

// Heroes REST endpoints
app.MapGet("/api/heroes", (IHeroGrainClient client) => client.GetAll())
	.WithTags("Heroes");

app.MapGet("/api/heroes/{id}", (string id, IHeroGrainClient client) => client.Get(id))
	.WithTags("Heroes");

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
using Heroes.Contracts;
using Heroes.Server;
using Heroes.Server.Infrastructure;
using Heroes.Server.Realtime;
using Heroes.Server.Sample;
using Heroes.GrainClients;
using Heroes.Server.Gql;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.UseHeroesServer(args, heroes =>
{
	heroes
		.ConfigureServices(services =>
		{
			services.AddAppGrains();
		})
		.ConfigureOrleans(siloBuilder =>
		{
			siloBuilder.UseSignalR(cfg =>
			{
				cfg.Configure((sb, signalrBuilderConfig) =>
				{
					sb.UseStorage(signalrBuilderConfig.StorageProvider, heroes.AppInfo, storeName: "SignalR");
				});
			});
		});
});

builder.WebHost.UseUrls("http://localhost:6600");

builder.Services.AddSingleton<IHeroService, HeroService>();
builder.Services.AddCustomAuthentication();
builder.Services.AddSignalR()
	.AddJsonProtocol(opts =>
	{
		opts.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		opts.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
	})
	.AddOrleans();

builder.Services.AddCors(o => o.AddPolicy("TempCorsPolicy", corsBuilder =>
{
	corsBuilder
		.SetIsOriginAllowed(_ => true)
		.AllowAnyMethod()
		.AllowAnyHeader()
		.AllowCredentials();
}));

builder.Services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);
builder.Services.AddAppClients();
builder.Services.AddAppGraphQL();
builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

if (app.Environment.IsDevelopment())
	app.UseDeveloperExceptionPage();

app.UseCors("TempCorsPolicy");
app.UseGraphQL("/graphql");
app.UseGraphQLPlayground("/", new()
{
	GraphQLEndPoint = "/graphql",
	SubscriptionsEndPoint = "/graphql",
});

app.UseRouting();
app.UseAuthorization();
app.MapHub<HeroHub>("/real-time/hero");
app.MapHub<UserNotificationHub>("/userNotifications");
app.MapControllers();

await app.RunAsync();

using Grace.DependencyInjection;
using GraphQL;
using Heroes.Core;
using Heroes.GrainClients;
using Heroes.Server.Gql;
using Heroes.Server.Infrastructure;
using Heroes.Server.Realtime;
using Heroes.Server.Sample;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Heroes.Server
{
	public class ApiStartup
	{
		private readonly IConfiguration _configuration;
		private readonly IAppInfo _appInfo;

		public ApiStartup(
			IConfiguration configuration,
			IAppInfo appInfo
		)
		{
			_configuration = configuration;
			_appInfo = appInfo;
		}

		public void ConfigureContainer(IInjectionScope scope)
		{

		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IHeroService, HeroService>();
			services.AddCustomAuthentication();
			services.AddSignalR()
				.AddJsonProtocol(opts =>
				{
					opts.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
					opts.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
				})
				.AddOrleans();


			services.AddCors(o => o.AddPolicy("TempCorsPolicy", builder =>
			{
				builder
					// .SetIsOriginAllowed((host) => true)
					.WithOrigins("http://localhost:4200")
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials()
					;
			}));

			// note: to fix graphql for .net core 3
			services.Configure<KestrelServerOptions>(options =>
			{
				options.AllowSynchronousIO = true;
			});

			services.AddAppClients();
			services.AddAppGraphQL();
			services.AddControllers()
			.AddNewtonsoftJson();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(
			IApplicationBuilder app,
			IWebHostEnvironment env
		)
		{
			app.UseCors("TempCorsPolicy");

			app.UseGraphQL("/graphql"); // url to host GraphQL endpoint
			app.UseGraphQLPlayground(
				"/", // url to host Playground at
				new()
				{
					GraphQLEndPoint = "/graphql", // url of GraphQL endpoint
					SubscriptionsEndPoint = "/graphql", // url of GraphQL endpoint
				});


			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHub<HeroHub>("/real-time/hero");
				endpoints.MapHub<UserNotificationHub>("/userNotifications");
				endpoints.MapControllers();
			});
		}
	}
}
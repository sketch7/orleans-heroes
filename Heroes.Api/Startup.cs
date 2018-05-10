using Heroes.Api.GraphQLCore;
using Heroes.Api.Infrastructure;
using Heroes.Api.Realtime;
using Heroes.Api.Sample;
using Heroes.Clients;
using Heroes.Contracts.Grains.Heroes;
using Heroes.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Heroes.Api
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IHeroService, HeroService>();
			var appInfo = new AppInfo(Configuration);
			services.AddSingleton<IAppInfo>(appInfo);
			services.AddCustomAuthentication();

			var clientBuilderContext = new ClientBuilderContext
			{
				Configuration = Configuration,
				AppInfo = appInfo,
				ConfigureClientBuilder = clientbuilder =>
					clientbuilder.ConfigureApplicationParts(x => x.AddApplicationPart(typeof(IHeroCollectionGrain).Assembly).WithReferences())
					.UseSignalR()
			};

			services.AddSignalR()
				.AddOrleans();

			services.UseOrleansClient(clientBuilderContext);
			services.AddHeroesClients();
			services.AddHeroesAppGraphQL();
			services.AddMvc();
			services.AddCors(o => o.AddPolicy("TempCorsPolicy", builder =>
			{
				builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials();
			}));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(
			IApplicationBuilder app,
			IHostingEnvironment env,
			ILoggerFactory loggerFactory
		)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.UseCors("TempCorsPolicy");
			app.SetGraphQLMiddleWare();
			//app.UseWebSockets();
			//app.UseGraphQLEndPoint<HeroesAppSchema>("/graphql");

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseGraphiQl();
			}

			app.UseSignalR(routes =>
			{
				routes.MapHub<HeroHub>("/real-time/hero");
				routes.MapHub<UserNotificationHub>("/userNotifications");
			});

			app.UseMvc();
		}
	}
}
using Grace.DependencyInjection;
using GraphiQl;
using Heroes.Clients;
using Heroes.Core;
using Heroes.Server.Gql;
using Heroes.Server.Infrastructure;
using Heroes.Server.Realtime;
using Heroes.Server.Sample;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;

namespace Heroes.Server
{
	public class ApiStartup
	{
		private readonly IConfiguration _configuration;
		private readonly IAppInfo _appInfo;

		public ApiStartup(
			IConfiguration configuration,
			IAppInfo appInfo,
			IClusterClient clusterClient
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

			//var clientBuilderContext = new ClientBuilderContext
			//{
			//	Configuration = Configuration,
			//	AppInfo = appInfo,
			//	ConfigureClientBuilder = clientBuilder =>
			//		clientBuilder.ConfigureApplicationParts(x => x.AddApplicationPart(typeof(IHeroCollectionGrain).Assembly).WithReferences())
			//		.UseSignalR()
			//};

			services.AddSignalR()
				.AddOrleans();

			services.AddCors(o => o.AddPolicy("TempCorsPolicy", builder =>
			{
				builder
					//.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader()
					.WithOrigins("http://localhost:4200")
					.AllowCredentials()
					;
			}));

			//services.UseOrleansClient(clientBuilderContext);
			services.AddCustomAuthentication();
			services.AddAppClients();
			services.AddAppGraphQL();
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(
			IApplicationBuilder app,
			IHostingEnvironment env
		)
		{
			app.UseCors("TempCorsPolicy");
			app.UseAppGraphQLMiddleware();
			//app.UseWebSockets();
			//app.UseGraphQLEndPoint<AppSchema>("/graphql");

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
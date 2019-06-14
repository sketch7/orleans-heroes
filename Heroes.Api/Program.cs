using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Heroes.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<ApiStartup>()
				.UseUrls("http://*:62551")
				.Build();
	}
}
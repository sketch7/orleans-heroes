using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
namespace Heroes.Server.Infrastructure
{
	public class CustomAuthenticationHandler : AuthenticationHandler<JwtBearerOptions>
	{
		public const string Authorization = "token";
		public const string TokenInvalid = "TokenInvalid";
		public static string SecretKey = "orleans-test";

		public CustomAuthenticationHandler(
			IOptionsMonitor<JwtBearerOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			ISystemClock clock
		) : base(options, logger, encoder, clock)
		{
		}

		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			string token = !string.IsNullOrEmpty(Request.Headers[Authorization])
				? Request.Headers[Authorization]
				: Request.Query[Authorization];

			if (string.IsNullOrEmpty(token))
				return AuthenticateResult.NoResult();

			var provider = await GetByKey(token);
			if (provider == null)
				return AuthenticateResult.Fail(TokenInvalid);

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, provider.Name),
				new Claim(ClaimTypes.NameIdentifier, provider.Name)
			};
			var claimsIdentity = new ClaimsIdentity(claims, SecretKey);
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			var authTicket = new AuthenticationTicket(claimsPrincipal, new AuthenticationProperties(), SecretKey);

			return AuthenticateResult.Success(authTicket);
		}

		private readonly List<AuthModel> _mockUsers = new List<AuthModel>
		{
			new AuthModel
			{
				Id = "cla",
				Name = "clayton",
				Key = "cla-key"
			},
			new AuthModel
			{
				Id = "ste",
				Name = "stephen",
				Key = "ste-key"
			},
		};

		public Task<AuthModel> GetByKey(string key) => Task.FromResult(_mockUsers.FirstOrDefault(x => x.Key == key));
	}

	public static class AuthServiceCollectionExtensions
	{
		public static void AddCustomAuthentication(this IServiceCollection services)
		{
			services.AddAuthentication(CustomAuthenticationHandler.SecretKey)
				.AddScheme<JwtBearerOptions, CustomAuthenticationHandler>(CustomAuthenticationHandler.SecretKey, null);

			services.AddAuthorization(auth =>
			{
				auth.AddPolicy(CustomAuthenticationHandler.SecretKey, builder =>
				{
					builder.AddAuthenticationSchemes(CustomAuthenticationHandler.SecretKey)
						.RequireAuthenticatedUser();
				});
			});
		}
	}

	public class AuthModel
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Key { get; set; }
	}
}
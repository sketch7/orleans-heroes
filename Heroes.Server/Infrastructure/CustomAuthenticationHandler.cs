using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Heroes.Server.Infrastructure;

public class CustomAuthenticationHandler(
	IOptionsMonitor<JwtBearerOptions> options,
	ILoggerFactory logger,
	UrlEncoder encoder
) : AuthenticationHandler<JwtBearerOptions>(options, logger, encoder)
{
	public const string Authorization = "token";
	public const string TokenInvalid = "TokenInvalid";
	public static string SecretKey = "orleans-test";

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		string? token = Request.Headers[Authorization];
		if (string.IsNullOrEmpty(token))
			token = Request.Query[Authorization];

		if (string.IsNullOrEmpty(token))
			return AuthenticateResult.NoResult();

		var provider = await GetByKey(token);
		if (provider == null)
			return AuthenticateResult.Fail(TokenInvalid);

		var claims = new List<Claim>
		{
			new(ClaimTypes.Name, provider.Name),
			new(ClaimTypes.NameIdentifier, provider.Name)
		};
		var claimsIdentity = new ClaimsIdentity(claims, SecretKey);
		var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
		var authTicket = new AuthenticationTicket(claimsPrincipal, new(), SecretKey);

		return AuthenticateResult.Success(authTicket);
	}

	private readonly List<AuthModel> _mockUsers =
	[
		new()
		{
			Id = "cla",
			Name = "clayton",
			Key = "cla-key"
		},
		new()
		{
			Id = "ste",
			Name = "stephen",
			Key = "ste-key"
		}
	];

	public Task<AuthModel?> GetByKey(string key) => Task.FromResult(_mockUsers.FirstOrDefault(x => x.Key == key));
}

public static class AuthServiceCollectionExtensions
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddAppAuth()
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

			return services;
		}
	}
}

public record AuthModel
{
	public required string Id { get; init; }
	public required string Name { get; init; }
	public required string Key { get; init; }
}
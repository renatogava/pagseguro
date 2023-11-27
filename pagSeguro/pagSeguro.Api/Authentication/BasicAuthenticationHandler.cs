using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace pagSeguro.Api.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(
                    AuthenticateResult.Fail("Missing Authorization"));
            }

            var authorizationHeader = Request.Headers["Authorization"].ToString();

            if (!authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                AuthenticateResult.Fail("Authorization Header does not start with Basic ");
            }

            var authBase64Decoded = Encoding.UTF8.GetString(
                Convert.FromBase64String(
                    authorizationHeader.Replace("Basic ", "", StringComparison.OrdinalIgnoreCase
                    ))
                );

            var authSplit = authBase64Decoded.Split(new[] { ':' }, 2);

            if (authSplit.Length != 2)
            {
                return Task.FromResult(
                    AuthenticateResult.Fail("Invalid Authorization format"));

            }

            var clientId = authSplit[0];
            var clientSecret = authSplit[1];

            if (clientId != "editoracontracorrente" || clientSecret != "hOXy8%waXdT*")
            {
                return Task.FromResult(
                    AuthenticateResult.Fail("Usuário ou senha inválidos"));
            }

            var client = new BasicAuthenticationClient
            {
                AuthenticationType = "Basic",
                IsAuthenticated = true,
                Name = clientId
            };

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(client, new[] { new Claim(ClaimTypes.Name, clientId) }));

            return Task.FromResult(
                AuthenticateResult.Success(
                    new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
        }
    }
}

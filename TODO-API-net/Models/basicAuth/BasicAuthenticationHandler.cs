using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using TODO_API_net.Models.basicAuth;
using UserAPI.Models;
using static TODO_API_net.Models.basicAuth.BasicAuthServices;
using System.Text.Encodings.Web;

namespace TODO_API_net.Models.basicAuth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IBasicAuthService _basicAuthService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IBasicAuthService basicAuthService
        ) : base(options, logger, encoder, clock)
        {
            _basicAuthService = basicAuthService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            if (!authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            var encodedCredentials = authorizationHeader.Substring("Basic ".Length).Trim();
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials)).Split(':');

            if (credentials.Length != 2)
            {
                return AuthenticateResult.Fail("Invalid Basic Authentication Credentials");
            }

            var username = credentials[0];
            var password = credentials[1];

            var user = await _basicAuthService.Authenticate(username, password);

            if (user == null)
            {
                return AuthenticateResult.Fail("Invalid username or password");
            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var identity = new ClaimsIdentity(claims, "Basic");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "BasicAuthentication");

            return AuthenticateResult.Success(ticket);
        }
    }
}

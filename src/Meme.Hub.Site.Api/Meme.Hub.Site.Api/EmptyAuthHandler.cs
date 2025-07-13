using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Meme.Hub.Site.Api
{
    public class EmptyAuthContext : ResultContext<AuthenticationSchemeOptions>
    {
        public EmptyAuthContext(HttpContext context, AuthenticationScheme scheme, AuthenticationSchemeOptions options)
            : base(context, scheme, options) { }
    }

    public class EmptyAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public EmptyAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            await Task.Yield(); // now we're async

            var principal = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), Scheme.Name));
            var context = new EmptyAuthContext(Context, Scheme, Options) { Principal = principal };

            context.Success();
            return context.Result;
        }
    }
}

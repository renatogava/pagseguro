using Microsoft.AspNetCore.Authorization;

namespace pagSeguro.Api.Authentication
{
    public class BasicAuthenticationAttribute : AuthorizeAttribute
    {
        public BasicAuthenticationAttribute()
        {
            AuthenticationSchemes = "Basic";
        }
    }
}

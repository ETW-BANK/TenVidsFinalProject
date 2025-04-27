
using System.Security.Claims;

namespace TenVids.Services.Extensions
{
    public static class ClaimExtension
    {
        public static string GetUserName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        }

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        public static string GetName(this ClaimsPrincipal claimsPrincipal)
        {
            var givenName = claimsPrincipal.FindFirst(ClaimTypes.GivenName)?.Value;

            if (string.IsNullOrEmpty(givenName))
            {
                
                Console.WriteLine("GivenName is not available. Falling back to Identity.Name.");

                givenName = claimsPrincipal.Identity?.Name ?? string.Empty;
            }

            return givenName ?? string.Empty;
        }



        public static string? GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}

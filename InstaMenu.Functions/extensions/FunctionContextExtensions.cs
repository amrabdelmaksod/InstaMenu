using Microsoft.Azure.Functions.Worker;
using System.Security.Claims;

namespace InstaMenuFunctions.extensions
{
    public static class FunctionContextExtensions
    {
        public static ClaimsPrincipal? GetUser(this FunctionContext context)
        {
            if (context.Items.TryGetValue("User", out var user) && user is ClaimsPrincipal principal)
                return principal;

            return null;
        }
    }
}
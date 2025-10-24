using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InstaMenuFunctions.Middlewares
{
    public class JwtMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly string _jwtSecret = "xrrKo7oTBExKQm6UdEPHKEgFq+dy0PBE8c/ncvYrwSY="; // استخدم IConfiguration لو حبيت

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var req = await context.GetHttpRequestDataAsync();

            if (req != null && req.Headers.TryGetValues("Authorization", out var authHeaders))
            {
                var bearer = authHeaders.FirstOrDefault(h => h.StartsWith("Bearer "));
                if (!string.IsNullOrWhiteSpace(bearer))
                {
                    var token = bearer["Bearer ".Length..];

                    var principal = ValidateToken(token);
                    if (principal != null)
                    {
                        context.Items["User"] = principal; // ✅ هنستخدمها في ctx.GetUser()
                    }
                }
            }

            await next(context);
        }

        private ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSecret);

                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
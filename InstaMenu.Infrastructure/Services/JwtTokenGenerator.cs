using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InstaMenu.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace InstaMenu.Infrastructure.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly string _secret;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiresDays;

        public JwtTokenGenerator()
        {
            _secret = Environment.GetEnvironmentVariable("JWT_SECRET") 
                ?? throw new InvalidOperationException("JWT_SECRET environment variable is required");
            
            _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "InstaMenu";
            _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "InstaMenuClient";
            
            if (int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRES_DAYS"), out var expiresDays))
            {
                _expiresDays = expiresDays;
            }
            else
            {
                _expiresDays = 7; // Default to 7 days
            }
        }

        public string GenerateToken(Guid merchantId, string name)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, merchantId.ToString()),
                new Claim(ClaimTypes.Name, name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_expiresDays),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
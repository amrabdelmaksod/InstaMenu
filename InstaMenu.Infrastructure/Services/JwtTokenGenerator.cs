using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InstaMenu.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace InstaMenu.Infrastructure.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly string _secret = "xrrKo7oTBExKQm6UdEPHKEgFq+dy0PBE8c/ncvYrwSY=";

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
                issuer: "InstaMenu",
                audience: "InstaMenuClient",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
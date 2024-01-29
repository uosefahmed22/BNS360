using BNS360.Core.Entities.Identity;
using BNS360.Core.Helpers.Enums;
using BNS360.Core.Helpers.Settings;
using BNS360.Core.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BNS360.Reposatory.Repositories.Authentication
{
    public class JwtGenerator : IJwtGenerator
    {
        private readonly IOptionsMonitor<JwtSettings> _jwtSettings;

        public JwtGenerator(IOptionsMonitor<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateJwt(AppUser user,string role )
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email ,user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId",user.Id),
                new Claim("roles",role),
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString())
            };
            var jwt = _jwtSettings.CurrentValue;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
               jwt.Key ?? throw new InvalidOperationException("secret key is null")));

            var token = new JwtSecurityToken(
                    claims: authClaims,
                    issuer: jwt.Issuer,
                    audience: jwt.Audience,
                    expires: DateTime.UtcNow.AddMinutes(jwt.Expires),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

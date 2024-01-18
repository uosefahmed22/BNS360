using BNS360.Core.Entities.Identity;
using BNS360.Core.Helpers.Settings;
using BNS360.Core.Services.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Reposatory.Repositories.Authentication
{
    public class JwtGenerator : IJwtGenerator
    {
        private readonly IOptionsMonitor<JwtSettings> _jwtSettings;

        public JwtGenerator(IOptionsMonitor<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateJwt(AppUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email ,user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId",user.Id),
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _jwtSettings.CurrentValue.Key ?? throw new InvalidOperationException("secret key is null")));

            var token = new JwtSecurityToken
                (
                    claims: authClaims,
                    issuer: _jwtSettings.CurrentValue.Issuer,
                    audience: _jwtSettings.CurrentValue.Audience,
                    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.CurrentValue.Expires),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

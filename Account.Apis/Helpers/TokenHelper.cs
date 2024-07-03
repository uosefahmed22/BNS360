using System.IdentityModel.Tokens.Jwt;

namespace Account.Apis.Helpers
{
    public class TokenHelper
    {
        public static Dictionary<string, string> GetTokenClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            return jwtToken.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);
        }
    }
}

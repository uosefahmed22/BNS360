
namespace BNS360.Core.Helpers.Settings
{
    public class JwtSettings
    {
        public required string Key { get; init; } 
        public required string Issuer { get; set; }
        public required string Audience {  get; set; }
        public float Expires { get; set; }
    }
}

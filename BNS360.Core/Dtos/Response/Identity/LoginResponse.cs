using BNS360.Core.Errors;

namespace BNS360.Core.Dtos.Response.Identity
{
    public class LoginResponse : ApiResponse
    {      
        public required string DisplayName { get; set; }
        public required string Email { get; set; }
        public string? JwtToken { get; set; }
      
    } 
}
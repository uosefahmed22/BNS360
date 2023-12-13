using System.ComponentModel.DataAnnotations;
namespace BNS360.Core.Dtos.Request.Identity
{
    public class LoginRequest
    {
        [EmailAddress]
        public required string Email { get; set; }
        [MinLength(8)]
        public required string Password { get; set; }
    }
}

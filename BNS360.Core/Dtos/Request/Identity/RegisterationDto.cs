using BNS360.Core.Helpers.Enums;
using System.ComponentModel.DataAnnotations;

namespace BNS360.Core.Dtos.Request.Identity
{
    public class RegisterationDto
    {

        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "Only Arabic and English letters, and spaces are allowed.")]
        public required string Name { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        [MinLength(8 , ErrorMessage = "Min allawed Length is 8 characters")]
        public required string Password { get; set; }

        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Invalid phone number format. Please enter a valid Egyptian phone number.")]
        public string? PhoneNumber { get; set; }
        [EnumDataType(typeof(UserType),ErrorMessage = "only 0 ,1 and 2 allawed")]
        public UserType UserType { get; set; }  
    }
}
using System.ComponentModel.DataAnnotations;

namespace BNS360.Core.Dtos.Request
{
    public class ContactDto
    {      
        [Required]
        public required string FirstPhoneNumber { get; set; }
        public string? SecoundPhoneNumber { get; set; }
        public string? ThirdPhoneNumber { get; set; }
        public required string EmailAddress { get; set; }
        public string? SiteUrl { get; set; }
    }
}
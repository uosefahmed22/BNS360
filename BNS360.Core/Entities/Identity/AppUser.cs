using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BNS360.Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        [Required]
        public required string Name { get; set; }
    }
}

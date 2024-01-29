using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Entities
{
    public class Contact : BaseEntity
    {
        [Phone]
        [Required]
        public required string FirstPhoneNumber { get; set; }
        [Phone]
        public string? SecoundPhoneNumber { get; set; }
        [Phone]
        public string? ThirdPhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? SiteUrl { get; set; }
        [Required]
        [ForeignKey("BusnissId")]
        public Guid BusnissId { get; set; }
    }
    
}

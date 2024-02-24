using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNS360.Core.Entities
{
    public class Location : BaseEntity
    {
      
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public required string Address { get; set; }
        [Required]
        [ForeignKey("BusnissId")]
        public Guid BusnissId { get; set; }
    }
}

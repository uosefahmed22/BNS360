using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNS360.Core.Entities
{
    public class Review : BaseEntity
    {
        [Range(0,5)]
        public float Rate { get; set; }
        public string? Comment { get; set; }
        [Required]
        [ForeignKey("BusnissId")]
        public Guid BusnissId { get; set; }
        public Guid? UserId { get; set; }
    }
}

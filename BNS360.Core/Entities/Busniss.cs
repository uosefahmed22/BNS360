using BNS360.Core.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNS360.Core.Entities
{
    public class Busniss : BaseEntity , IHaveReviews
    {
        public required string Name { get; set; }
        public required string About { get; set; }
        public string? ProfilePictureUrl {  get; set; }
        [Required]
        public Guid UserId { get; set; } 
        [Required]
        [ForeignKey("CategoryId")]
        public Guid CategoryId { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public Location? Location { get; set; } 
        public ICollection<WorkTime> WorkTime { get; set; } = new List<WorkTime>();
        public Contact? ContactInfo { get; set; } 
    }
}

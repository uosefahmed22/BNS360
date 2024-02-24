using System.ComponentModel.DataAnnotations;

namespace BNS360.Core.Dtos.Request
{
    public class ReviewRequest
    {
        [Range(0, 5)]
        public int Rate { get; set; }
        public string? Comment { get; set; }
        public Guid BusnissId { get; set; }
    }
}
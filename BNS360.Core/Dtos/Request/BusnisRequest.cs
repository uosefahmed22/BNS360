
using BNS360.Core.Helpers;
using System.ComponentModel.DataAnnotations;

namespace BNS360.Core.Dtos.Request
{
    public record BusnisRequest(Guid CategoryId) : BaseBusnisRequest;

    public record BusnisUpdateRequest(Guid id) : BaseBusnisRequest;

    public abstract record BaseBusnisRequest
    {
        [MaxLength(100)]
        public required string Name { get; set; }
        [MaxLength(500)]
        public required string About { get; set; }
        public required LocationDto Location { get; set; }
        [ListSize(maxSize:7,minSize:1)]
        public List<WorkTimeDto> WorkTime { get; set; } = new();
        public required ContactDto ContactInfo { get; set; }
    }

}

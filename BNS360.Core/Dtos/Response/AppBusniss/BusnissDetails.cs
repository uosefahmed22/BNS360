using BNS360.Core.Entities;
using BNS360.Core.Errors;

namespace BNS360.Core.Dtos.Response.AppBusniss
{
    public class BusnissDetails : ApiResponse
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required Contact ContactInfo { get; set; }
        public required List<WorkTime> WorkTime { get; set; }
        public required Location LocationInfo { get; set; }
        public List<Review>? Reviews { get; set; }
        public ReviewSummary ReviewSummary { get; set; }
    }
}
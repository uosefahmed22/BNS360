using BNS360.Core.Entities;
using BNS360.Core.Errors;


namespace BNS360.Core.Dtos.Response.AppBusniss
{
    public class BusnissDetails : ApiResponse
    {
        public Guid Id { get; set; }
        public required string NameAR { get; set; }
        public required string Description { get; set; }
        public string? NameENG { get; set; }
        public string? DescriptionENG { get; set; }
        public required Contact ContactInfo { get; set; }
        public required List<WorkTime> WorkTime { get; set; }
        public required Location LocationInfo { get; set; }
        public required ReviewSummary ReviewSummary { get; set; }
        public string? ProfilePicture { get; set; } 
        public IReadOnlyList<string>? AlbumPictures { get; set; } 
        public required string Category { get; set; }
    }
}
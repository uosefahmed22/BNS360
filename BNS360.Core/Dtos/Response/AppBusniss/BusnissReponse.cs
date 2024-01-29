
namespace BNS360.Core.Dtos.Response.AppBusniss
{
    public class BusnissReponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public required ReviewSummary ReviewSummary { get; set; } = new(0, 0);
    }
}
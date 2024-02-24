
using System.Text.Json.Serialization;

namespace BNS360.Core.Dtos.Response.AppBusniss
{
    public class BusnissReponse
    {
        public required Guid Id { get; set; }
        public required string NameAR { get; set; }
        public required string DescriptionAR { get; set; }
        public string? NameENG { get; set; }
        public string? DescriptionENG { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public required ReviewSummary ReviewSummary { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Distance { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace BNS360.Core.Entities
{
    public class Category : BaseEntity
    { 
        public required string Name { get; set; }
        public required string PictureUrl { get; set; }
        [JsonIgnore]
        public ICollection<Business>? Busnisses { get; set; }
    }
}

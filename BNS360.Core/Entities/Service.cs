namespace BNS360.Core.Entities
{
    public class Service : BaseEntity
    {
        public required string Name { get; set; }
        public string? CoverPictureUrl { get; set; }
    }
}
namespace BNS360.Core.Dtos.Request
{
    public class ContactDto
    {
        public required List<string> PhoneNumbers { get; set; }
        public required string EmailAddress { get; set; }
        public string? SiteUrl { get; set; }
    }
}
namespace BNS360.Core.Dtos.Request
{
    public class LocationDto
    {
        public required decimal Latitude { get; set; }
        public required decimal Longitude { get; set; }
        public required string Address { get; set; }

    }
}

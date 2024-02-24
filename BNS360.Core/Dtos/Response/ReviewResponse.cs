namespace BNS360.Core.Dtos.Response;

public class ReviewResponse
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public string? UserProfilePicture { get; set; }
    public string? Comment { get; set; }
    public float Rate { get; set; }
    public DateTime LastModefied { get; set; }  
}

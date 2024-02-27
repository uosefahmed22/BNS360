using BNS360.Core.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNS360.Core.Entities;

public class Business : MainEntity
{
    public required string NameAR { get; set; }
    public string? NameENG { get; set; }
    public required string AboutAR { get; set; }
    public string? AboutENG { get; set; }
    public string? ProfilePictureUrl {  get; set; }
    public string? AlbumUrl {  get; set; }
    public IList<Review>? Reviews { get; set; }

    [Required]
    public Guid UserId { get; set; } 
    [Required]
    [ForeignKey("CategoryId")]
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }  
    public Location? Location { get; set; } 
    public IList<WorkTime> WorkTime { get; set; } = new List<WorkTime>();
    public Contact? ContactInfo { get; set; } 
}

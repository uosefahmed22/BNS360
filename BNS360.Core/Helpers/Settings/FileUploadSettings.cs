using System.Text.Json.Serialization;

namespace BNS360.Core.Helpers.Settings;

public class FileUploadSettings
{
    public int Size {  get; set; }
    [JsonPropertyName("extensions")]
    public required string[] Extensions { get; set; }
}

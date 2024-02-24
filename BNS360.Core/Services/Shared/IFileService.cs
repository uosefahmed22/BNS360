using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BNS360.Core.Services.Shared;

public interface IFileService
{
    string? GetAbsoluteFilePath(string? filePath);
    Task<string> Comprease(IFormFile file, string outputPath);
    IReadOnlyList<string>? GetAlbumPictures(string? albumPath);


    /// <summary>
    /// Stores an IFormFile in a directory.
    /// </summary>
    /// <param name="file">The file to store.</param>
    /// <param name="uploadDirectory">The directory to store the file in.</param>
    /// <param name="fileName">The file name without extension. If it's null, a random file name will be generated.</param>
    /// <returns>A string representing the file path where the file is stored.</returns>
    /// <exception cref="InValidExtentionException">Thrown when the file extension is not valid.</exception>
    Task<string> StoreAsync(IFormFile file, string uploadDirectory, string? fileName = null);
}

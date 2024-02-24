using BNS360.Core.CustemExceptions;
using BNS360.Core.Helpers.Settings;
using BNS360.Core.Services.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.IO.Compression;

namespace BNS360.Reposatory.Repositories.Repositories;

public class FileService(IHttpContextAccessor contextAccessor,
    IOptionsMonitor<FileUploadSettings> fileUploadSettings) : IFileService
{
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;
    private readonly FileUploadSettings _fileUploadSettings = fileUploadSettings.CurrentValue;
    public async Task<string> Comprease(IFormFile file, string outputPath)
    {
        outputPath = Path.ChangeExtension(outputPath, ".gz");
        using (var compreasedFileStream = File.Create(outputPath))
        {
            using(GZipStream gzStream = new GZipStream(compreasedFileStream, CompressionMode.Compress))
            {
                using(Stream fileStream = file.OpenReadStream())
                {
                     await fileStream.CopyToAsync(gzStream);    
                }
            }

        }
        return outputPath;
    }

    public string? GetAbsoluteFilePath(string? filePath)
    {
        if (filePath == null)
            return default(string);

        string relativePath = filePath.Split("wwwroot")[1];
        return
            $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}" +
            $"/{relativePath}";

    }
    public IReadOnlyList<string>? GetAlbumPictures(string? albumPath)
    {
        if (string.IsNullOrEmpty(albumPath))
            return null;

        var result = Directory.GetFiles(albumPath!)
            .Select(fileUrl => this.GetAbsoluteFilePath(fileUrl))
            .Where(filePath => filePath is not null)
            .ToList();

        return result.AsReadOnly()!;
    }

    public async Task<string> StoreAsync(IFormFile file, string uploadDirectory, string? fileName = null)
    {
        var fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (!_fileUploadSettings.Extensions.Contains(fileExtension))
        {
            throw new InValidExtentionException(_fileUploadSettings);
        }

        if (string.IsNullOrEmpty(fileName))
        {
            fileName = Path.GetRandomFileName() + fileExtension;
        }

        var filePath = Path.Combine(uploadDirectory, fileName + fileExtension); 

        var size = file.Length / 1024;  

        if (size > _fileUploadSettings.Size)
        {
            filePath = await Comprease(file, filePath);
        }
        else
        {
            using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
        }
        return filePath;
    }
}

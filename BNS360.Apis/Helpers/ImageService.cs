using BNS360.Core.IServices;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace BNS360.Apis.Helpers;

public sealed class ImageService : IImageService
{
    private const long MaximumFileSize = 5 * 1024 * 1024;

    private static readonly IReadOnlyDictionary<string, string[]> AllowedTypes =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = ["image/jpeg"],
            [".jpeg"] = ["image/jpeg"],
            [".png"] = ["image/png"]
        };

    private readonly Cloudinary _cloudinary;
    private readonly ILogger<ImageService> _logger;

    public ImageService(Cloudinary cloudinary, ILogger<ImageService> logger)
    {
        _cloudinary = cloudinary ?? throw new ArgumentNullException(nameof(cloudinary));
        _logger = logger;
    }

    public async Task<Tuple<int, string>> UploadImageAsync(
        IFormFile imageFile,
        CancellationToken cancellationToken = default)
    {
        if (imageFile.Length is <= 0 or > MaximumFileSize)
        {
            return Tuple.Create(0, "Image size must be between 1 byte and 5 MB.");
        }

        var extension = Path.GetExtension(imageFile.FileName);
        if (!AllowedTypes.TryGetValue(extension, out var allowedContentTypes)
            || !allowedContentTypes.Contains(imageFile.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            return Tuple.Create(0, "Only valid JPG, JPEG, and PNG images are allowed.");
        }

        await using var stream = imageFile.OpenReadStream();
        if (!await HasValidSignatureAsync(stream, extension, cancellationToken))
        {
            return Tuple.Create(0, "The uploaded file content is not a valid image.");
        }

        stream.Position = 0;
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(Path.GetFileName(imageFile.FileName), stream),
            UseFilename = false,
            UniqueFilename = true,
            Overwrite = false
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
        if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK
            || uploadResult.SecureUrl is null)
        {
            _logger.LogWarning(
                "Cloudinary image upload failed with status {StatusCode}",
                uploadResult.StatusCode);
            return Tuple.Create(0, "The image could not be uploaded.");
        }

        return Tuple.Create(1, uploadResult.SecureUrl.AbsoluteUri);
    }

    public async Task DeleteImageAsync(
        string imageUrl,
        CancellationToken cancellationToken = default)
    {
        var publicId = GetPublicIdFromUrl(imageUrl);
        cancellationToken.ThrowIfCancellationRequested();
        var deletionResult = await _cloudinary.DestroyAsync(
            new DeletionParams(publicId) { Invalidate = true });

        if (deletionResult.Result is not ("ok" or "not found"))
        {
            _logger.LogWarning(
                "Cloudinary image deletion returned {Result} for public id {PublicId}",
                deletionResult.Result,
                publicId);
        }
    }

    private static async Task<bool> HasValidSignatureAsync(
        Stream stream,
        string extension,
        CancellationToken cancellationToken)
    {
        var header = new byte[8];
        var bytesRead = await stream.ReadAsync(header, cancellationToken);

        if (extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
        {
            byte[] pngSignature = [137, 80, 78, 71, 13, 10, 26, 10];
            return bytesRead >= pngSignature.Length
                && header.AsSpan(0, pngSignature.Length).SequenceEqual(pngSignature);
        }

        return bytesRead >= 3
            && header[0] == 0xFF
            && header[1] == 0xD8
            && header[2] == 0xFF;
    }

    private static string GetPublicIdFromUrl(string url) =>
        Path.GetFileNameWithoutExtension(new Uri(url).AbsolutePath);
}

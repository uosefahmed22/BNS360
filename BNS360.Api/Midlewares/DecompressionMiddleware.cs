using System.IO.Compression;

namespace BNS360.Api.Midlewares;

public class DecompressionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DecompressionMiddleware> _logger;  

    public DecompressionMiddleware(RequestDelegate next, ILogger<DecompressionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation($"Successfully hit InvokeAsync within {this.ToString()}");
        var originalBody = context.Response.Body;

        using (var newBody = new MemoryStream())
        {
            context.Response.Body = newBody;

            await _next(context);

            // Check if the response has a .gz file
            if (context.Response.ContentType == "application/x-gzip")
            {
                _logger.LogInformation($"Caught .gz file response");

                newBody.Seek(0, SeekOrigin.Begin);

                try
                {
                    using (var decompressionStream = new GZipStream(newBody, CompressionMode.Decompress))
                    {
                        // Set proper content type for decompressed content
                        context.Response.ContentType = "image/jpeg"; // Example, adjust as needed
                        await decompressionStream.CopyToAsync(originalBody);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error decompressing .gz file: {ex.Message}");
                    // Handle error accordingly
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }
            }
            else
            {
                _logger.LogInformation($"Does not catch .gz file response. The response content type is {context.Response.ContentType}");

                newBody.Seek(0, SeekOrigin.Begin);
                await newBody.CopyToAsync(originalBody);
            }
        }

        _logger.LogInformation($"New Body Content Type: {context.Response.ContentType}");
    }


}
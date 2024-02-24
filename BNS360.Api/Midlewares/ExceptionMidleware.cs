using BNS360.Core.CustemExceptions;
using BNS360.Core.Errors;

namespace BNS360.Api.Midlewares;

public class ExeptionMiddleWares
{
    public ExeptionMiddleWares(RequestDelegate next, ILogger<ExeptionMiddleWares> logger, IHostEnvironment env)
    {
        _next = next;
        _Logger = logger;
        _env = env;
    }

    private RequestDelegate _next {  get; set; }
    private ILogger<ExeptionMiddleWares> _Logger { get; set; }
    private IHostEnvironment _env { get; set; }
    

    public async Task Invoke(HttpContext context)
    {            
        try
        {
            var authHeader = context.Request.Headers.Authorization;
            await _next(context);
        }
        catch(InValidExtentionException ex)
        {
            await TakeAction(ex, context,StatusCodes.Status415UnsupportedMediaType);
        }
        catch (Exception ex)
        {
            await TakeAction(ex, context,StatusCodes.Status500InternalServerError);
            
        }
    }
    private async Task TakeAction(Exception ex, HttpContext context, int statusCode)
    {
        _Logger.LogError(ex, ex.Message);
        context.Response.StatusCode = statusCode;

        var response = _env.IsDevelopment() ?
            new ApiExceptionResponse(statusCode, ex.Message, ex.StackTrace) :
            new ApiResponse(statusCode);

        await context.Response.WriteAsJsonAsync(response);
    }
}
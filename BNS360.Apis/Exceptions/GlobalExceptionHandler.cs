using System.Diagnostics;
using BNS360.Core.Errors;
using Microsoft.EntityFrameworkCore;

namespace BNS360.Apis.Exceptions;

public sealed class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        RequestDelegate next,
        ILogger<GlobalExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (OperationCanceledException)
            when (httpContext.RequestAborted.IsCancellationRequested)
        {
            _logger.LogDebug(
                "Request was cancelled by the client for {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }
        catch (Exception exception)
        {
            if (httpContext.Response.HasStarted)
            {
                _logger.LogError(
                    exception,
                    "The response has already started for {Method} {Path}",
                    httpContext.Request.Method,
                    httpContext.Request.Path);
                throw;
            }

            await HandleExceptionAsync(httpContext, exception);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext httpContext,
        Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "The request is invalid."),
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "The requested resource was not found."),
            UnauthorizedAccessException => (
                StatusCodes.Status403Forbidden,
                "You do not have permission to access this resource."),
            DbUpdateConcurrencyException => (
                StatusCodes.Status409Conflict,
                "The request conflicts with the current state of the resource."),
            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.")
        };

        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Unhandled exception for {Method} {Path}. TraceId: {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                traceId);
        }
        else
        {
            _logger.LogWarning(
                "Handled {ExceptionType} for {Method} {Path}. TraceId: {TraceId}",
                exception.GetType().Name,
                httpContext.Request.Method,
                httpContext.Request.Path,
                traceId);
        }

        httpContext.Response.StatusCode = statusCode;

        var response = new ApiResponse(
            statusCode,
            message,
            new { TraceId = traceId });

        await httpContext.Response.WriteAsJsonAsync(
            response,
            httpContext.RequestAborted);
    }
}

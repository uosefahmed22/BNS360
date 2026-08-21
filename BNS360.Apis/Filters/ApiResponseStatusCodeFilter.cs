using BNS360.Core.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BNS360.Apis.Filters;

public sealed class ApiResponseStatusCodeFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(
        ResultExecutingContext context,
        ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult
            {
                Value: ApiResponse { StatusCode: int statusCode }
            } objectResult
            && statusCode is >= 100 and <= 599)
        {
            objectResult.StatusCode = statusCode;
        }

        await next();
    }
}

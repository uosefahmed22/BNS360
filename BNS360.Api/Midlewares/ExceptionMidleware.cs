using BNS360.Core.Errors;
using Microsoft.AspNetCore.Connections;
using System.Net;
using System.Security.AccessControl;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BNS360.Api.Midlewares
{
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
            catch (Exception ex)
            {
                _Logger.LogError(ex,ex.Message);
                int statusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.StatusCode = statusCode;   
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var response = _env.IsDevelopment() ?
                    new ApiExceptionResponse(statusCode, ex.Message,ex.Source?.ToString()) :
                    new ApiResponse(statusCode);

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(response);
               
                
            }
        }
    
    }
}


using System.Text.Json.Serialization;

namespace BNS360.Core.Errors
{
    public class ApiResponse
    {
        [JsonIgnore]
        public int StatusCode { get; set; }

        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }

        public ApiResponse()
        {
            
        }
        public ApiResponse(int statusCode, string? errorMessage = null)
        {
            StatusCode = statusCode;
            Message = errorMessage ?? GetDefaultErrorMessage(statusCode);
        }

        private string? GetDefaultErrorMessage(int statusCode)
        {
            return statusCode switch
            {
                200 => "Succeeded",
                400 => "Bad Request",
                401 => "Unauthorized: You are not authorized to access this resource.",
                403 => "Forbidden: Access to this resource is forbidden.",
                404 => "Not Found: The requested resource was not found.",
                405 => "Method Not Allowed: The HTTP method is not allowed for the requested resource.",
                408 => "Request Timeout: The server timed out waiting for the request.",
                429 => "Too Many Requests: The user has sent too many requests in a given amount of time.",
                500 => "Internal Server Error: Something went wrong on the server.",
                502 => "Bad Gateway: The server received an invalid response from the upstream server.",
                503 => "Service Unavailable: The server is currently unable to handle the request due to maintenance or overload.",
                504 => "Gateway Timeout: The server didn't receive a timely response from the upstream server.",
                _ => null,
            };
        }

    }
}


using System.Text.Json.Serialization;

namespace BNS360.Core.Errors
{
    public class ApiExceptionResponse : ApiResponse
    {
        public ApiExceptionResponse(int statusCode, string? errorMessage = null,string? details = null) : base(statusCode, errorMessage)
        {
            Details = details;
        }
        [JsonPropertyOrder(0)]
        public String? Details { get; set; }



    }
}

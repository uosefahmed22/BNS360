using BNS360.Core.Errors;
using Microsoft.AspNetCore.Http;

namespace BNS360.Core.Dtos.Response;

public class PagenationResponse<TResult> : ApiResponse
{
    public PagenationResponse()
        : base(StatusCodes.Status200OK)
    {
            
    }
    public int CurrentPage { get; set; }
    public int TotalCount { get; set; }
    public List<TResult> Items { get; set; } = new List<TResult>();
}

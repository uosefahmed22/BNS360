using BNS360.Core.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BNS360.Api.Controllers
{
    [Route("errors/{code}")]
    
    public class ErrorController : ApiBaseController
    {
        [HttpGet]
        public IActionResult Error(int code)
            => new ObjectResult(new ApiResponse(code));
        
    }
}

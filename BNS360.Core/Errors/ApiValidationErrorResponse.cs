using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Errors
{
    public class ApiValidationErrorResponse : ApiResponse
    {
        public Dictionary<string,List<string>?>? Errors;

        public ApiValidationErrorResponse(int statusCode, string? errorMessage = null
            , Dictionary<string, List<string>?>? errors = null)
            : base(statusCode, errorMessage)
        {
        }
    }
}

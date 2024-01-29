using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Errors
{
    public class ApiValidationResponse : ApiResponse
    {
        public ApiValidationResponse(Dictionary<string, List<string>> errors)
        {
            Errors = errors;
        }

        public Dictionary<string, List<string>> Errors { get; set; }

    }
}

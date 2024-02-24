using BNS360.Core.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Dtos.Response.AppBusniss
{
    public class FavoriteBusnissResponse : ApiResponse
    {
        public required string Name { get; set; }
        public required string Category { get; set; }
        public string? PictureUrl { get; set; }
        public required ReviewSummary ReviewSummary { get; set; }
    }
}

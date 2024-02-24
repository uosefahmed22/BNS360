using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Dtos.Request
{
    public class CurrentLocation
    {
        public required decimal Latitude { get; set; }
        public required decimal Longitude { get; set; }
    }
}

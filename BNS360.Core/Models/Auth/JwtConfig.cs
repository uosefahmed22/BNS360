using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Models.Auth
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public string ExpirationInMinutes { get; set; }
    }
}

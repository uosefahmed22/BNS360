using BNS360.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Services.Authentication
{
    public interface IJwtGenerator
    {
       string GenerateJwt(AppUser user);
    }
}

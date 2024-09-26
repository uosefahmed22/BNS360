using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Dto
{
    public class UserDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public List<string> UserRole { get; set; }

    }
}

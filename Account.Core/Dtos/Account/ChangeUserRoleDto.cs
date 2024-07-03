using Account.Core.Enums.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Core.Dtos.Account
{
    public class ChangeUserRoleDto
    {
        public string UserId { get; set; }
        public UserRoleEnum NewRole { get; set; }
    }
}

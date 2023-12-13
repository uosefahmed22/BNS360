using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Dtos.Request.Identity
{
    public class ResetPassword
    {
        [EmailAddress]
        public required string Email { get; set; }
        
        public required string Password { get; set; }
        [Compare("Password",ErrorMessage = "Password Not Confirmed")]
        public required string ConfirmedPassword { get; set; }    
    }
    
}

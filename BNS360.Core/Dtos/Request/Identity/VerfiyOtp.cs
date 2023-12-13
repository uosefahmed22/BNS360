using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Dtos.Request.Identity
{
    public class VerfiyOtp
    {
        [EmailAddress] 
        public required string Email { get; set; }
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "only digits allawed")]
        [Length(6,6, ErrorMessage = "OTP must be 6 digits")]
        public required string Otp { get; set; }
    }
}

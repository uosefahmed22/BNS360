using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.IServices.Auth
{
    public interface IOtpService
    {
        public string GenerateOtp(string email);
        public string? VerifyOtpAndCreateResetToken(string email, string otp);
        public bool ConsumeResetToken(string email, string resetToken);
    }
}

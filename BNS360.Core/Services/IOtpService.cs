
namespace BNS360.Core.Services
{
    public interface IOtpService
    {
        public string GenerateOtp(string email);
        public bool IsValidOtp(string email,string otp);
    }
}
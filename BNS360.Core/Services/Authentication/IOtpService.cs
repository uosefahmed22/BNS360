namespace BNS360.Core.Services.Authentication
{
    public interface IOtpService
    {
        public string GenerateOtp(string email);
        public bool IsValidOtp(string email, string otp);
    }
}
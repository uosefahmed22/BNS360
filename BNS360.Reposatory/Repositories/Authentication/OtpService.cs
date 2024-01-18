using BNS360.Core.Services.Authentication;
using Microsoft.Extensions.Caching.Memory;
using OtpNet;

namespace BNS360.Reposatory.Repositories.Authentication
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;

        public OtpService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenerateOtp(string email)
        {
            var key = KeyGeneration.GenerateRandomKey(32);
            StoreKeyInCache(email, key);
            var totp = new Totp(key, step: 3600);
            return totp.ComputeTotp();
        }

        public bool IsValidOtp(string email, string otp)
        {
            var key = RetrieveKeyFromCache(email);
            if (key is null)
                // Key not found in cache, OTP validation fails
                return false;

            var totp = new Totp(key, step: 3600);
            var isValiddOtp = totp.VerifyTotp(otp, out _, new VerificationWindow(1, 1));
            if (!isValiddOtp)
                return false;

            _cache.Remove(email);
            _cache.Set(email, true, TimeSpan.FromMinutes(10));

            return isValiddOtp;
        }


        private void StoreKeyInCache(string email, byte[] key)
            =>
            // Store the key in the memory cache with a specific key name
            _cache.Set(email, key, TimeSpan.FromMinutes(60));


        private byte[]? RetrieveKeyFromCache(string email)
        {
            // Retrieve the key associated with the email from the memory cache
            if (_cache.TryGetValue(email, out byte[]? key))
                return key;

            return null;
        }
    }
}
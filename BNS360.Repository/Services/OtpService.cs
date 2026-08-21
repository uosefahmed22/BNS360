using System.Security.Cryptography;
using System.Text;
using BNS360.Core.IServices.Auth;
using Microsoft.Extensions.Caching.Memory;

namespace BNS360.Repository.Services;

public sealed class OtpService : IOtpService
{
    private const int MaximumAttempts = 5;
    private static readonly object CacheLock = new();
    private readonly IMemoryCache _cache;

    public OtpService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public string GenerateOtp(string email)
    {
        var normalizedEmail = NormalizeEmail(email);
        var otp = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        var salt = RandomNumberGenerator.GetBytes(16);
        var entry = new OtpEntry(Hash(otp, salt), salt, 0);

        lock (CacheLock)
        {
            _cache.Set(OtpCacheKey(normalizedEmail), entry, TimeSpan.FromMinutes(5));
        }

        return otp;
    }

    public string? VerifyOtpAndCreateResetToken(string email, string otp)
    {
        var normalizedEmail = NormalizeEmail(email);

        lock (CacheLock)
        {
            var cacheKey = OtpCacheKey(normalizedEmail);
            if (!_cache.TryGetValue(cacheKey, out OtpEntry? entry) || entry is null)
            {
                return null;
            }

            if (!CryptographicOperations.FixedTimeEquals(entry.Hash, Hash(otp, entry.Salt)))
            {
                var failedEntry = entry with { FailedAttempts = entry.FailedAttempts + 1 };
                if (failedEntry.FailedAttempts >= MaximumAttempts)
                {
                    _cache.Remove(cacheKey);
                }
                else
                {
                    _cache.Set(cacheKey, failedEntry, TimeSpan.FromMinutes(5));
                }

                return null;
            }

            _cache.Remove(cacheKey);

            var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            _cache.Set(
                ResetTokenCacheKey(resetToken),
                normalizedEmail,
                TimeSpan.FromMinutes(10));

            return resetToken;
        }
    }

    public bool ConsumeResetToken(string email, string resetToken)
    {
        var normalizedEmail = NormalizeEmail(email);
        var cacheKey = ResetTokenCacheKey(resetToken);

        lock (CacheLock)
        {
            if (!_cache.TryGetValue(cacheKey, out string? tokenEmail)
                || !string.Equals(tokenEmail, normalizedEmail, StringComparison.Ordinal))
            {
                return false;
            }

            _cache.Remove(cacheKey);
            return true;
        }
    }

    private static string NormalizeEmail(string email) => email.Trim().ToUpperInvariant();

    private static string OtpCacheKey(string normalizedEmail) => $"password-reset:otp:{normalizedEmail}";

    private static string ResetTokenCacheKey(string resetToken)
    {
        var tokenHash = SHA256.HashData(Encoding.UTF8.GetBytes(resetToken));
        return $"password-reset:token:{Convert.ToHexString(tokenHash)}";
    }

    private static byte[] Hash(string value, byte[] salt)
    {
        var valueBytes = Encoding.UTF8.GetBytes(value);
        var input = new byte[salt.Length + valueBytes.Length];
        Buffer.BlockCopy(salt, 0, input, 0, salt.Length);
        Buffer.BlockCopy(valueBytes, 0, input, salt.Length, valueBytes.Length);
        return SHA256.HashData(input);
    }

    private sealed record OtpEntry(byte[] Hash, byte[] Salt, int FailedAttempts);
}

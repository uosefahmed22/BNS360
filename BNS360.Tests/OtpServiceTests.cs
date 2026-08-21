using BNS360.Repository.Services;
using Microsoft.Extensions.Caching.Memory;

namespace BNS360.Tests;

public sealed class OtpServiceTests
{
    [Fact]
    public void ValidOtp_CreatesSingleUseResetTokenBoundToEmail()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new OtpService(cache);
        var otp = service.GenerateOtp("user@example.com");

        var resetToken = service.VerifyOtpAndCreateResetToken("USER@example.com", otp);

        Assert.NotNull(resetToken);
        Assert.False(service.ConsumeResetToken("other@example.com", resetToken!));
        Assert.True(service.ConsumeResetToken("user@example.com", resetToken!));
        Assert.False(service.ConsumeResetToken("user@example.com", resetToken!));
        Assert.Null(service.VerifyOtpAndCreateResetToken("user@example.com", otp));
    }

    [Fact]
    public void Otp_IsInvalidatedAfterFiveFailedAttempts()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new OtpService(cache);
        var otp = service.GenerateOtp("user@example.com");
        var wrongOtp = otp == "000000" ? "999999" : "000000";

        for (var attempt = 0; attempt < 5; attempt++)
        {
            Assert.Null(service.VerifyOtpAndCreateResetToken("user@example.com", wrongOtp));
        }

        Assert.Null(service.VerifyOtpAndCreateResetToken("user@example.com", otp));
    }
}

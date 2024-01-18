using BNS360.Core.Dtos;
using BNS360.Core.Dtos.Request.Identity;
using BNS360.Core.Dtos.Response.Identity;
using BNS360.Core.Errors;

namespace BNS360.Core.Services.Authentication
{
    public interface IAuthService
    {
        Task<ApiResponse> Register(RegisterationDto dto, Func<string, string, string> generateCallBackUrl);
        Task<ApiResponse> Login(LoginRequest dto);
        Task<bool> ConfirmUserEmailAsync(string userId, string emailConfirmationToken);
        Task<ApiResponse> ForgetPassword(string email);
        ApiResponse VerfiyOtp(VerfiyOtp dto);
        Task<ApiResponse> ResetPasswordAsync(ResetPassword dto);
    }
}
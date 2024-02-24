using BNS360.Core.CustemExceptions;
using BNS360.Core.Dtos;
using BNS360.Core.Dtos.Request.Identity;
using BNS360.Core.Dtos.Response.Identity;
using BNS360.Core.Entities.Identity;
using BNS360.Core.Errors;
using BNS360.Core.Helpers.Enums;
using BNS360.Core.Services.Authentication;
using BNS360.Core.Services.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
namespace BNS360.Reposatory.Repositories.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly IMemoryCache _cache;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IFileService _fileService;
        public AuthService(UserManager<AppUser> userManager,
            IEmailService emailService, IOtpService otpService,
            IMemoryCache cache, IJwtGenerator jwtGenerator, IFileService fileService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _otpService = otpService;
            _cache = cache;
            _jwtGenerator = jwtGenerator;
            _fileService = fileService;
        }


        public async Task<ApiResponse> Register(RegisterationDto dto, Func<string, string, string> generateCallBackUrl)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is not null)
                return new ApiResponse(409, "user already exists");

            user = new AppUser
            {
                Name = dto.Name,
                Email = dto.Email,
                EmailConfirmed = false,
                UserName = dto.Email,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new ApiResponse
                (
                    500,
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }

            string role = dto.UserType switch
            {
                UserType.Default => nameof(UserType.Default),
                UserType.BusinssOwner => nameof(UserType.BusinssOwner),
                UserType.ServiceProvider => nameof(UserType.ServiceProvider),
                _ => throw new Exception("Role not defined for"),
            };

            await _userManager.AddToRoleAsync(user, role);

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            //Console.WriteLine($"Email Token is: {emailConfirmationToken}");

            var callBackUrl = generateCallBackUrl(emailConfirmationToken, user.Id);

            var emailBody = $"<h1>Dear {user.Name}! Welcome To BNS360.</h1><p>Please <a href='{callBackUrl}'>Click Here</a> To Confirm Your Email.</p>";

            await _emailService.SendEmailAsync(user.Email, "Email Confirmation", emailBody);

            return new ApiResponse(200);
        }

        public async Task<bool> ConfirmUserEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            ItemNotFoundException.ThrowIfNull(user, nameof(user));

            var confirmed = await _userManager.ConfirmEmailAsync(user, token);

            if (confirmed.Succeeded)
                return true;

            return false;
        }

        public async Task<ApiResponse> LoginAsync(LoginRequest dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            ItemNotFoundException.ThrowIfNull(user, nameof(user));
            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                return new ApiResponse(400, $"email or password might be wrong");

            if (!user.EmailConfirmed)
                return new ApiResponse(409) { Message = "Email Not Confirmed" };

            var roles = await _userManager.GetRolesAsync(user);

            return new LoginResponse()
            {
                StatusCode = 200,
                DisplayName = user.Name,
                ProfilePicture = _fileService.GetAbsoluteFilePath(user.profilePictureUrl),
                Email = user.Email!,
                JwtToken = _jwtGenerator.GenerateJwt(user,roles.First())
            };
        }
    
        public async Task<ApiResponse> ForgetPassword(string email)
        {
            var user = await _userManager.Users.Where(u => u.Email == email)
                .Select(u => new { u.Email, u.EmailConfirmed, u.Name })
                .FirstOrDefaultAsync();

            ItemNotFoundException.ThrowIfNull(user,nameof(user));   

            var otp = _otpService.GenerateOtp(email);
            await _emailService.SendEmailAsync(email,
                "Verfication Code", $"Dear{user.Name}</br>use this is your rest password code <h1>{otp}</h1>keep it safe and do`nt share it");

            return new ApiResponse()
            {
                Message = email
            };

        }

        public ApiResponse VerfiyOtp(VerfiyOtp dto)
            => _otpService.IsValidOtp(dto.Email, dto.Otp) ? 
            new ApiResponse(200, dto.Email) 
            : new ApiResponse(400, "Invalid Otp");

        public async Task<ApiResponse> ResetPasswordAsync(ResetPassword dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            ItemNotFoundException.ThrowIfNull(user, nameof(user));
            if (_cache.TryGetValue(dto.Email, out bool validOtp))
            {
                if (validOtp)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var result = await _userManager.ResetPasswordAsync(user, token, dto.Password);

                    return result.Succeeded ? new ApiResponse(200) : new ApiResponse(500);
                }

            }
            return new ApiResponse(400, "You have not verfied email address");
        }
    }
}
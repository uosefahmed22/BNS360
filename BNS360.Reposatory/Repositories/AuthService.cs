using BNS360.Core.Dtos;
using BNS360.Core.Dtos.Request.Identity;
using BNS360.Core.Dtos.Response.Identity;
using BNS360.Core.Entities.Identity;
using BNS360.Core.Errors;
using BNS360.Core.Helpers.Enums;
using BNS360.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
namespace BNS360.Reposatory.Repositories
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly IMemoryCache _cache;
        public AuthService(UserManager<AppUser> userManager,
            IConfiguration config,
            IEmailService emailService,
            IOtpService otpService,
            IMemoryCache cache)
        {
            _userManager = userManager;
            _config = config;
            _emailService = emailService;
            _otpService = otpService;
            _cache = cache;
        }

        public async Task<bool> ConfirmUserEmailAsync(string userId, string emailConfirmationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return false;
            //Console.WriteLine($"Email Token is: {emailConfirmationToken}");
            //Console.WriteLine($"{user.Id}\n{userId} \n{user.Name}\n{user.Email}\n{user.EmailConfirmed}");
            var confirmed = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);

            if (confirmed.Succeeded)
                return true;

            return false;
        }
        public async Task<ApiResponse> ForgetPassword(string email)
        {
            var user = await _userManager.Users.Where(u => u.Email == email)
                .Select(u => new {Email = u.Email,EmailConfirmed = u.EmailConfirmed,Name = u.Name})
                .FirstOrDefaultAsync();

    

            var otp = _otpService.GenerateOtp(email);
            await _emailService.SendEmailAsync(email, "Verfication Code", $"Dear{user.Name}</br>use this is your rest password code <h1>{otp}</h1>keep it safe and do`nt share it");

            return new ApiResponse()
            {
                ErrorMessage = "if email exists ,message is sent to your email"
            };

        }

        public ApiResponse VerfiyOtp(VerfiyOtp dto) 
            => _otpService.IsValidOtp(dto.Email, dto.Otp) ? new ApiResponse(200,dto.Email) : new ApiResponse(400,"Invalid Otp");
        public async Task<ApiResponse> Login(LoginRequest dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return new ApiResponse (400);

            if (!user.EmailConfirmed)
                return new ApiResponse()
                {
                    ErrorMessage = "Email Not Confirmed",
                 
                };
               

            //var roles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email ,user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId",user.Id),               
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString())               
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config["JWT:Key"] ?? throw new InvalidOperationException("secret key is null")));

            var token = new JwtSecurityToken
                (
                    claims: authClaims,
                    issuer: _config["JWT:issuer"],
                    audience: _config["JWT:audience"],
                    expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["JWT:expires"]!)),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            return new LoginResponse()
            {
                DisplayName = user.Name,
                Email = user.Email!,
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token)
            };
         
            
        }
        public async Task<ApiResponse> Register(RegisterationDto dto, Func<string, string, string> generateCallBackUrl)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is not null)
                return new ApiResponse (404, "user already exists");

            user = new AppUser
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
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

            string role = ((UserType)dto.UserType) switch
            {
                UserType.Default => "Default",
                UserType.BusinssOwner => "BusinessOwner",
                UserType.ServiceProvider => "ServiceProvider",
                _ => throw new Exception("Role not defined for"),
            };
            
            await _userManager.AddToRoleAsync(user, role);

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            //Console.WriteLine($"Email Token is: {emailConfirmationToken}");

            var callBackUrl = generateCallBackUrl(emailConfirmationToken, user.Id);
            var emailBody = $"<h1>Dear {user.Name}! Welcome To BNS360.</h1><p>Please <a href='{callBackUrl}'>Click Here</a> To Confirm Your Email.</p>";
            try
            {
                await _emailService.SendEmailAsync(user.Email, "Email Confirmation", emailBody);
            }
            catch 
            {
                return new ApiResponse(500,"unabel to send email confirmation please try leater");
            }

            return new ApiResponse(200);
        }
        public async Task<ApiResponse> ResetPasswordAsync(ResetPassword dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);            
            if (user == null) 
                return new ApiResponse(404,$"no user registerd with this {dto.Email}");

            if (_cache.TryGetValue(dto.Email, out bool validOtp))
            {
                if (validOtp)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var result = await _userManager.ResetPasswordAsync(user, token, dto.Password);

                    return result.Succeeded ? new ApiResponse(200) : new ApiResponse(500);
                }
                               
            }
                     
            return new ApiResponse(400,"You have not verfied email address");
        }
    }
}
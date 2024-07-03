using Account.Apis.Errors;
using Account.Core.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Security;
using Account.Core.Dtos.Account;
using Account.Core.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Account.Core.Services.Auth;
using Account.Core.Enums.Auth;

namespace Account.Reposatory.Services.Authentications
{
    public class AccountService : IAccountService
    {
        #region MyRegion
        private readonly UserManager<AppUser> _userManager;
        private readonly MailSettings _mailSettings;
        private readonly ITokenService _TokenService;
        private readonly IOtpService _otpService;
        private readonly IMemoryCache _cache;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountService(UserManager<AppUser> userManager,
            IOptionsMonitor<MailSettings> options,
            ITokenService tokenService,
            IOtpService otpService,
            IMemoryCache cache,
            SignInManager<AppUser> signInManager
            )
        {
            _userManager = userManager;
            _mailSettings = options.CurrentValue;
            _TokenService = tokenService;
            _otpService = otpService;
            _cache = cache;
            _signInManager = signInManager;
        }
        #endregion

        public async Task<ApiResponse> RegisterAsync(Register dto, Func<string, string, string> generateCallBackUrl)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is not null)
            {
                return new ApiResponse(400, "User with this email already exists.");
            }

            user = new AppUser
            {
                DisplayName = dto.DisplayName,
                Email = dto.Email,
                UserName = dto.Email.Split('@')[0],
                UserRole = (int)dto.UserRole,
                EmailConfirmed = false
            };
            //error here when adding password
            var Result = await _userManager.CreateAsync(user, dto.Password);

            if (!Result.Succeeded)
            {
                return new ApiResponse(400, "Something went wrong with the data you entered");
            }

            var roleName = GetUserRoleName(dto.UserRole);
            await _userManager.AddToRoleAsync(user, roleName);

            var EmailConfirmation = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callBackUrl = generateCallBackUrl(EmailConfirmation, user.Id);
            var emailBody = $"<h1>Dear {user.UserName}! Welcome To BNS360.</h1><p>Please <a href='{callBackUrl}'>Click Here</a> To Confirm Your Email.</p>";

            await SendEmailAsync(user.Email, "Email Confirmation", emailBody);

            return new ApiResponse(200, "Email verification has been sent to your email successfully. Please verify it!");

        }
        public static string GetUserRoleName(UserRoleEnum role)
        {
            return role switch
            {
                UserRoleEnum.User => "User",
                UserRoleEnum.BussinesOwner => "BussinesOwner",
                UserRoleEnum.ServiceProvider => "ServiceProvider",
                UserRoleEnum.Admin => "Admin",
                _ => "Unknown",
            };
        }
        public async Task<ApiResponse> LoginAsync(Login dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new ApiResponse(400, "User not found.");
            }

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return new ApiResponse(400, "Incorrect email or password.");
            }

            if (!user.EmailConfirmed)
            {
                return new ApiResponse(400, "Email not confirmed. Please check your email inbox to verify your email address.");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();


            return new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Role = (UserRoleEnum)user.UserRole,
                id = user.Id,
                Token = await _TokenService.CreateTokenAsync(user)
            };
        }
        private UserRoleEnum GetRoleEnum(string role)
        {
            return role switch
            {
                "User" => UserRoleEnum.User,
                "BussinesOwner" => UserRoleEnum.BussinesOwner,
                "ServiceProvider" => UserRoleEnum.ServiceProvider,
                "Admin" => UserRoleEnum.Admin,
            };
        }
        public async Task<ApiResponse> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email address is required.", nameof(email));
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ApiResponse(400, "User not found.");
            }

            var otp = _otpService.GenerateOtp(email);

            try
            {
                await SendEmailAsync(email,
                    "Verification Code",
                    $"Dear {user.UserName},<br/> Use this code to reset your password: <strong>{otp}</strong>. Keep it safe and do not share it with anyone.");

                return new ApiResponse(200, "Password reset email sent successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, "An unexpected error occurred while sending the password reset email. Please try again later.");
            }
        }
        public ApiResponse VerfiyOtp(VerifyOtp dto)
        {
            var isValidOtp = _otpService.IsValidOtp(dto.Email, dto.Otp);

            if (!isValidOtp)
            {
                return new ApiResponse(400, "Invalid OTP.");
            }
            return new ApiResponse(200, "Valid");
        }
        public async Task<ApiResponse> ResetPasswordAsync(ResetPassword dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return new ApiResponse(400, "User not found.");
            }

            if (!_cache.TryGetValue(dto.Email, out bool validOtp) || !validOtp)
            {
                return new ApiResponse(400, "You have not verified your email addres(OTP).");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, dto.Password);

            if (result.Succeeded)
            {
                return new ApiResponse(200, "Password reset successfully.");
            }
            else
            {
                return new ApiResponse(500, "Failed to reset password.");
            }
        }
        public async Task<ApiResponse> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return new ApiResponse(400, "User not found.");
            }

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return new ApiResponse(200, "Password changed successfully.");
            }
            else
            {
                return new ApiResponse(500, "Failed to change password.");
            }
        }
        public async Task<bool> ConfirmUserEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var confirmed = await _userManager.ConfirmEmailAsync(user, token);

            return confirmed.Succeeded;
        }
        public async Task SendEmailAsync(string To, string Subject, string Body, CancellationToken Cancellation = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailSettings.DisplayedName, _mailSettings.Email));
            message.To.Add(new MailboxAddress("", To));
            message.Subject = Subject;

            message.Body = new TextPart("html")
            {
                Text = Body
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(_mailSettings.SmtpServer, _mailSettings.Port,
                    SecureSocketOptions.StartTls, Cancellation);
                await client.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password, Cancellation);
                await client.SendAsync(message, Cancellation);
                await client.DisconnectAsync(true, Cancellation);
            }
        }
        public async Task<ApiResponse> RegisterForAdminAsync(RegisterForAdmin dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is not null)
            {
                return new ApiResponse(400, "User with this email already exists.");
            }

            user = new AppUser
            {
                DisplayName = dto.DisplayName,
                Email = dto.Email,
                UserName = dto.Email.Split('@')[0],
                UserRole = (int)dto.UserRole,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                return new ApiResponse(400, "Failed to create user.");
            }

            var roleName = GetUserRoleName(dto.UserRole);
            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
            {
                return new ApiResponse(400, "Failed to add user to role.");
            }

            return new ApiResponse(200, "Admin created successfully.");
        }
        public async Task<ApiResponse> LoginForAdminAsync(LoginForAdmin dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new ApiResponse(400, "User not found.");
            }

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return new ApiResponse(400, "Incorrect email or password.");
            }

            var token = await _TokenService.CreateTokenAsync(user);

            return new ApiResponse(200, "Login successful.", new AdminDto
            {
                id = user.Id,
                Token = token
            });
        }
        public async Task<ApiResponse> ResendConfirmationEmailAsync(string email, Func<string, string, string> generateCallBackUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new ApiResponse(400, "User with this email does not exist.");
            }

            if (user.EmailConfirmed)
            {
                return new ApiResponse(400, "Email is already confirmed.");
            }

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callBackUrl = generateCallBackUrl(emailConfirmationToken, user.Id);
            var emailBody = $"<h1>Dear {user.UserName}! Welcome To BNS360.</h1><p>Please <a href='{callBackUrl}'>Click Here</a> To Confirm Your Email.</p>";

            await SendEmailAsync(user.Email, "Email Confirmation", emailBody);

            return new ApiResponse(200, "Email verification has been resent to your email successfully. Please verify it!");
        }
    }
}

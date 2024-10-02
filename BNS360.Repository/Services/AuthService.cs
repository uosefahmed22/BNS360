using BNS360.Core.Errors;
using BNS360.Core.IServices.Auth;
using BNS360.Core.Models.Auth;
using BNS360.Repository.Data;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Repository.Services
{
    public class AuthService : IAuthService
    {
        #region
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbContext;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IMemoryCache _cache;
        private readonly JwtConfig _JwtConfig;
        private readonly IOtpService _otpService;
        private readonly MailSettings _mailSettings;

        public AuthService(UserManager<AppUser> userManager,
            IOptionsMonitor<JwtConfig> optionsMonitor,
            IMemoryCache cache,
            RoleManager<IdentityRole> roleManager,
            IOptionsMonitor<MailSettings> options,
            TokenValidationParameters tokenValidationParameters,
            IOtpService otpService,
            AppDbContext dbContext
                         )

        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _otpService = otpService;
            _tokenValidationParameters = tokenValidationParameters;
            _cache = cache;
            _JwtConfig = optionsMonitor.CurrentValue;
            _mailSettings = options.CurrentValue;
        }
        #endregion
        public async Task<ApiResponse> RigsterAsync(Register model, Func<string, string, string> generateCallBackUrl)
        {
            var exsistingUser = await _userManager.FindByEmailAsync(model.Email);
            if (exsistingUser != null)
            {
                return new ApiResponse
                {
                    StatusCode = 400,
                    Message = "البريد الإلكتروني مستخدم بالفعل"
                };
            }
            var newUser = new AppUser
            {
                FullName = model.Fullname,
                Email = model.Email,
                UserName = model.Email,
                EmailConfirmed = false
            };


            var isCreated = await _userManager.CreateAsync(newUser, model.Password);
            if (isCreated.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }
                await _userManager.AddToRoleAsync(newUser, "User");
                var jwtToken = await GenerateJwt(newUser);
                var EmailConfirmation = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var callBackUrl = generateCallBackUrl(EmailConfirmation, newUser.Id);
                var emailBody = $"<h1>عزيزي {newUser.FullName}</h1><p>شكرا لتسجيلك في موقعنا</p><p>لتأكيد حسابك اضغط على الرابط التالي</p><a href='{callBackUrl}'>اضغط هنا</a>";

                if (string.IsNullOrEmpty(newUser.Email))
                {
                    return new ApiResponse
                    {
                        StatusCode = 400,
                        Message = "حدث خطأ أثناء تسجيل المستخدم"
                    };
                }
                await SendEmailAsync(newUser.Email, "تأكيد البريد الإلكتروني", emailBody);
                return new ApiResponse(200, "الرجاء تأكيد البريد الإلكتروني الخاص بك");
            }
            else
            {
                return new ApiResponse
                {
                    StatusCode = 400,
                    Message = "حدث خطأ أثناء تسجيل المستخدم"
                };
            }

        }
        public async Task<ApiResponse> LoginAsync(Login model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return new ApiResponse(404, "البريد الإلكتروني أو كلمة المرور غير صحيحة");
                }
                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    return new ApiResponse(400, "البريد الإلكتروني أو كلمة المرور غير صحيحة");
                }
                if (!user.EmailConfirmed)
                {
                    return new ApiResponse(400, "الرجاء تأكيد البريد الإلكتروني الخاص بك");
                }
                var jwtToken = await GenerateJwt(user);
                return new ApiResponse(200, "تم تسجيل الدخول بنجاح", jwtToken);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }
        public async Task<ApiResponse> ChangePasswordAsync(ChangePassword model, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ApiResponse(404, "المستخدم غير موجود");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return new ApiResponse(200, "تم تغيير كلمة المرور بنجاح");
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return new ApiResponse(400, "حدث خطأ أثناء تغيير كلمة المرور", errors);
        }
        public async Task<bool> ConfirmUserEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }
        public async Task<ApiResponse> ForgetPassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new ApiResponse { StatusCode = 400, Message = "البريد الإلكتروني غير موجود" };
                }
                var otp = _otpService.GenerateOtp(email);
                var emailBody = $"<h1>كود التحقق</h1><p>كود التحقق الخاص بك هو {otp}</p>";
                await SendEmailAsync(email,
                       "كود التحقق",
                       $"كود التحقق الخاص بك هو: {otp}هذا الرمز صالح لمدة 5 دقائق فقط.");
                return new ApiResponse(200, "تم إرسال رمز التحقق إلى بريدك الإلكتروني");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }
        public async Task<ApiResponse> ResendConfirmationEmailAsync(string email, Func<string, string, string> generateCallBackUrl)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new ApiResponse { StatusCode = 400, Message = "البريد الإلكتروني غير موجود" };
                }
                var EmailConfirmation = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callBackUrl = generateCallBackUrl(EmailConfirmation, user.Id);
                var emailBody = $"<h1>عزيزي {user.FullName}</h1><p>شكرا لتسجيلك في موقعنا</p><p>لتأكيد حسابك اضغط على الرابط التالي</p><a href='{callBackUrl}'>اضغط هنا</a>";
                await SendEmailAsync(user.Email, "تأكيد البريد الإلكتروني", emailBody);
                return new ApiResponse(200, "الرجاء تأكيد البريد الإلكتروني الخاص بك");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }
        public async Task<ApiResponse> ResetPasswordAsync(ResetPassword model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return new ApiResponse(400, " المستخدم غير موجود.");
                }

                if (!_cache.TryGetValue(model.Email, out bool isOtpValid) || !isOtpValid)
                {
                    return new ApiResponse(400, "الرمز غير صالح.");
                }

                var isOldPasswordEqualNew = await _userManager.CheckPasswordAsync(user, model.Password);
                if (isOldPasswordEqualNew)
                {
                    return new ApiResponse(400, "كلمة المرور الجديدة يجب أن تكون مختلفة عن كلمة المرور القديمة.");
                }

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, model.Password);
                if (resetResult.Succeeded)
                {
                    return new ApiResponse(200, "لقد تم تغيير كلمة المرور بنجاح.");
                }

                var errorMessages = string.Join(", ", resetResult.Errors.Select(e => e.Description));
                return new ApiResponse(500, $"حدث خطأ أثناء تغيير كلمة المرور: {errorMessages}");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }
        public ApiResponse VerfiyOtp(VerifyOtp dto)
        {
            try
            {
                var isValidOtp = _otpService.IsValidOtp(dto.Email, dto.Otp);
                if (!isValidOtp)
                {
                    return new ApiResponse(400, "رمز التحقق غير صالح.");
                }
                return new ApiResponse(200, "رمز التحقق صالح.");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }

        //Handle Token
        public async Task<ApiResponse> RefreshToken([FromBody] TokenRequest model)
        {
            try
            {
                var result = await ValidateTokenAndRefreshJwt(model);
                if (result == null)
                {
                    return new ApiResponse(400, "غير مصرح لك بالوصول لهذا التوكن");
                }
                if (!result.Result)
                {
                    return new ApiResponse(400, $"{string.Join(", ", result.Errors)}");
                }
                return new ApiResponse(200, "تم تحديث التوكن بنجاح", result);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }

        }
        public async Task<ApiResponse> RevokeToken([FromBody] TokenRequest model)
        {
            try
            {
                var result = await ValidateTokenAndRefreshJwt(model);
                if (result == null)
                {
                    return new ApiResponse
                    {
                        Message = "غير مصرح لك بالوصول لهذا التوكن",
                        StatusCode = 401
                    };
                }
                var storedToken = await _dbContext.RefreshTokens.
                    FirstOrDefaultAsync(x => x.Token == model.RefreshToken);
                if (storedToken == null)
                {
                    return new ApiResponse
                    {
                        Message = "الرمز غير موجود",
                        StatusCode = 404
                    };
                }
                storedToken.IsRevoked = true;
                _dbContext.RefreshTokens.Update(storedToken);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse
                {
                    Message = "تم إلغاء التوكن بنجاح",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }

        //Helper Methods
        private async Task<AuthResult> GenerateJwt(AppUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_JwtConfig.Secret);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim("UserId" , user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtKey = _JwtConfig.Secret;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = GenerateRefreshToken()
            };
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
            return new AuthResult
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                Result = true
            };
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private async Task<AuthResult> ValidateTokenAndRefreshJwt(TokenRequest model)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                _tokenValidationParameters.ValidateLifetime = false;

                var tokenInVerification = jwtTokenHandler.ValidateToken(model.Token, _tokenValidationParameters, out var validatemodelken);
                if (tokenInVerification == null)
                {
                    return new AuthResult
                    {
                        Result = false,
                        Errors = new List<string> { "غير مصرح لك بالوصول لهذا التوكن" }
                    };
                }

                var claims = tokenInVerification.Claims.ToDictionary(c => c.Type, c => c.Value);

                // Check token expiration
                if (long.TryParse(claims[JwtRegisteredClaimNames.Exp], out var utcExpiryDate))
                {
                    var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
                    if (expiryDate > DateTime.UtcNow)
                    {
                        return new AuthResult
                        {
                            Result = false,
                            Errors = new List<string> { "التوكن مازال صالح" }
                        };
                    }
                }

                // Check refresh token from DB
                var storemodelken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == model.RefreshToken);
                if (storemodelken == null || storemodelken.Used || storemodelken.IsRevoked || storemodelken.ExpiryDate < DateTime.UtcNow)
                {
                    return new AuthResult
                    {
                        Result = false,
                        Errors = new List<string> { "التوكن غير صالح" }
                    };
                }

                // Verify the JTI (JWT ID)
                if (storemodelken.JwtId != claims[JwtRegisteredClaimNames.Jti])
                {
                    return new AuthResult
                    {
                        Result = false,
                        Errors = new List<string> { "التوكن لم تتطابق " }
                    };
                }

                // Mark token as used and save changes
                storemodelken.Used = true;
                _dbContext.RefreshTokens.Update(storemodelken);
                await _dbContext.SaveChangesAsync();

                // Generate new JWT
                var user = await _userManager.FindByIdAsync(storemodelken.UserId);
                return await GenerateJwt(user);
            }
            catch (SecurityTokenException ex)
            {
                return new AuthResult
                {
                    Result = false,
                    Errors = new List<string> { "التوكن غير صالح", ex.Message }
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Result = false,
                    Errors = new List<string> { "حدث خطأ أثناء تحديث التوكن", ex.Message }
                };
            }
        }
        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dateTimeVal;
        }
        private async Task SendEmailAsync(string To, string Subject, string Body, CancellationToken Cancellation = default)
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
    }
}

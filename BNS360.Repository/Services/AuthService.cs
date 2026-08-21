using BNS360.Core.Errors;
using BNS360.Core.IServices.Auth;
using BNS360.Core.Models.Auth;
using BNS360.Repository.Data;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly JwtConfig _JwtConfig;
        private readonly IOtpService _otpService;
        private readonly MailSettings _mailSettings;

        public AuthService(UserManager<AppUser> userManager,
            IOptionsMonitor<JwtConfig> optionsMonitor,
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
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new ApiResponse(401, "البريد الإلكتروني أو كلمة المرور غير صحيحة");
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return new ApiResponse(401, "تعذر تسجيل الدخول. حاول مرة أخرى لاحقا");
            }

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                await _userManager.AccessFailedAsync(user);
                return new ApiResponse(401, "البريد الإلكتروني أو كلمة المرور غير صحيحة");
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            if (!user.EmailConfirmed)
            {
                return new ApiResponse(403, "الرجاء تأكيد البريد الإلكتروني الخاص بك");
            }

            var jwtToken = await GenerateJwt(user);
            return new ApiResponse(200, "تم تسجيل الدخول بنجاح", jwtToken);
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
                await _dbContext.RefreshTokens
                    .Where(x => x.UserId == user.Id && !x.Invalidated)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.Invalidated, true));
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
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var otp = _otpService.GenerateOtp(email);
                await SendEmailAsync(email,
                    "كود التحقق",
                    $"كود التحقق الخاص بك هو: {otp}. هذا الرمز صالح لمدة 5 دقائق فقط.");
            }

            return new ApiResponse(200, "إذا كان البريد مسجلا فسيتم إرسال رمز التحقق إليه");
        }
        public async Task<ApiResponse> ResendConfirmationEmailAsync(string email, Func<string, string, string> generateCallBackUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && !user.EmailConfirmed)
            {
                var EmailConfirmation = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callBackUrl = generateCallBackUrl(EmailConfirmation, user.Id);
                var emailBody = $"<h1>عزيزي {user.FullName}</h1><p>شكرا لتسجيلك في موقعنا</p><p>لتأكيد حسابك اضغط على الرابط التالي</p><a href='{callBackUrl}'>اضغط هنا</a>";
                await SendEmailAsync(user.Email!, "تأكيد البريد الإلكتروني", emailBody);
            }

            return new ApiResponse(200, "إذا كان الحساب موجودا وغير مؤكد فسيتم إرسال رسالة التأكيد");
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

                if (!_otpService.ConsumeResetToken(model.Email, model.ResetToken))
                {
                    return new ApiResponse(400, "رمز إعادة تعيين كلمة المرور غير صالح أو انتهت مدته.");
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
                    await _dbContext.RefreshTokens
                        .Where(x => x.UserId == user.Id && !x.Invalidated)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(x => x.Invalidated, true));
                    return new ApiResponse(200, "لقد تم تغيير كلمة المرور بنجاح.");
                }

                var errorMessages = string.Join(", ", resetResult.Errors.Select(e => e.Description));
                return new ApiResponse(500, $"حدث خطأ أثناء تغيير كلمة المرور: {errorMessages}");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ApiResponse VerfiyOtp(VerifyOtp dto)
        {
            try
            {
                var resetToken = _otpService.VerifyOtpAndCreateResetToken(dto.Email, dto.Otp);
                if (resetToken == null)
                {
                    return new ApiResponse(400, "رمز التحقق غير صالح.");
                }
                return new ApiResponse(200, "رمز التحقق صالح.", new { ResetToken = resetToken });
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Handle Token
        public async Task<ApiResponse> RefreshToken([FromBody] TokenRequest model)
        {
            var result = await RotateRefreshTokenAsync(model);
            if (!result.Result)
            {
                return new ApiResponse(401, "التوكن غير صالح");
            }

            return new ApiResponse(200, "تم تحديث التوكن بنجاح", result);
        }

        public async Task<ApiResponse> RevokeToken([FromBody] TokenRequest model)
        {
            try
            {
                var principal = ValidateAccessToken(model.Token, validateLifetime: false);
                var jwtId = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
                var userId = principal.FindFirstValue("UserId");

                if (string.IsNullOrWhiteSpace(jwtId) || string.IsNullOrWhiteSpace(userId))
                {
                    return new ApiResponse(401, "التوكن غير صالح");
                }

                var revokedCount = await _dbContext.RefreshTokens
                    .Where(x => x.TokenHash == HashRefreshToken(model.RefreshToken)
                        && x.JwtId == jwtId
                        && x.UserId == userId
                        && !x.IsRevoked)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.IsRevoked, true));

                if (revokedCount != 1)
                {
                    return new ApiResponse(401, "التوكن غير صالح أو تم إلغاؤه مسبقا");
                }

                return new ApiResponse(200, "تم إلغاء التوكن بنجاح");
            }
            catch (SecurityTokenException)
            {
                return new ApiResponse(401, "التوكن غير صالح");
            }
        }

        //Helper Methods
        private async Task<AuthResult> GenerateJwt(AppUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_JwtConfig.Secret);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim("UserId" , user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? throw new InvalidOperationException("User email is missing.")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString("O"))
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtKey = _JwtConfig.Secret;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _JwtConfig.Issuer,
                Audience = _JwtConfig.Audience,
                Expires = DateTime.UtcNow.AddMinutes(_JwtConfig.ExpirationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var rawRefreshToken = GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                TokenHash = HashRefreshToken(rawRefreshToken)
            };
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
            return new AuthResult
            {
                Token = jwtToken,
                RefreshToken = rawRefreshToken,
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
        private async Task<AuthResult> RotateRefreshTokenAsync(TokenRequest model)
        {
            try
            {
                var principal = ValidateAccessToken(model.Token, validateLifetime: false);
                var jwtId = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
                var userId = principal.FindFirstValue("UserId");

                if (string.IsNullOrWhiteSpace(jwtId) || string.IsNullOrWhiteSpace(userId))
                {
                    return FailedAuthResult("التوكن غير صالح");
                }

                var storedToken = await _dbContext.RefreshTokens
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.TokenHash == HashRefreshToken(model.RefreshToken));

                if (storedToken == null
                    || storedToken.Used
                    || storedToken.IsRevoked
                    || storedToken.Invalidated
                    || storedToken.ExpiryDate <= DateTime.UtcNow
                    || storedToken.JwtId != jwtId
                    || storedToken.UserId != userId)
                {
                    return FailedAuthResult("التوكن غير صالح");
                }

                await using var transaction = await _dbContext.Database.BeginTransactionAsync();

                var consumedCount = await _dbContext.RefreshTokens
                    .Where(x => x.Id == storedToken.Id
                        && !x.Used
                        && !x.IsRevoked
                        && !x.Invalidated
                        && x.ExpiryDate > DateTime.UtcNow)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.Used, true));

                if (consumedCount != 1)
                {
                    await transaction.RollbackAsync();
                    return FailedAuthResult("التوكن تم استخدامه بالفعل");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    await transaction.RollbackAsync();
                    return FailedAuthResult("المستخدم غير موجود");
                }

                var result = await GenerateJwt(user);
                await transaction.CommitAsync();
                return result;
            }
            catch (SecurityTokenException)
            {
                return FailedAuthResult("التوكن غير صالح");
            }
        }

        private ClaimsPrincipal ValidateAccessToken(string token, bool validateLifetime)
        {
            var validationParameters = _tokenValidationParameters.Clone();
            validationParameters.ValidateLifetime = validateLifetime;

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken
                || !string.Equals(jwtToken.Header.Alg, SecurityAlgorithms.HmacSha256, StringComparison.Ordinal))
            {
                throw new SecurityTokenException("Invalid token algorithm.");
            }

            return principal;
        }

        private static AuthResult FailedAuthResult(string error) => new()
        {
            Result = false,
            Errors = new[] { error }
        };

        private static string HashRefreshToken(string refreshToken)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
            return Convert.ToHexString(hash);
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
                var smtpPassword = _mailSettings.SmtpServer.Equals(
                    "smtp.gmail.com",
                    StringComparison.OrdinalIgnoreCase)
                    ? string.Concat(_mailSettings.Password.Where(character => !char.IsWhiteSpace(character)))
                    : _mailSettings.Password;
                await client.AuthenticateAsync(_mailSettings.Email, smtpPassword, Cancellation);
                await client.SendAsync(message, Cancellation);
                await client.DisconnectAsync(true, Cancellation);
            }
        }
    }
}

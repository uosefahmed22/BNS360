using BNS360.Core.Dtos.Request.Identity;
using BNS360.Core.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BNS360.Api.Controllers
{
    public class AuthController : ApiBaseController
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
       
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterationDto dto)
        {
         
            var result = await _authService.Register(dto, GenerateCallBackUrl);

            if (result.StatusCode == 409)
                return Conflict(result);
            else if (result.StatusCode == 500)
                return StatusCode(500, result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest dto)
        {

            var result = await _authService.LoginAsync(dto);

            if (result.StatusCode == 400)
                return BadRequest(result);

            else if (result.StatusCode == 409)
                return Conflict(result);

            return Ok(result);
        }

        [HttpPost("forgetPassword")]
        public async Task<IActionResult> OtpRequest([FromHeader][EmailAddress]string email)
        {
            return Ok(await _authService.ForgetPassword(email));  
        }

        [HttpPost("verfiyOtp")]
        public IActionResult VerfiyOtp(VerfiyOtp dto)
        {

            var result = _authService.VerfiyOtp(dto);
            return result.StatusCode == 200 ? Ok(result) : BadRequest(result);
        }

        [HttpPut("resetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword dto)
        {

            var result = await _authService.ResetPasswordAsync(dto);

            if (result.StatusCode == 500)
                return StatusCode(500, result);

            else if (result.StatusCode == 400)
                return BadRequest(result);

            return Ok(result);            
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmUserEmail(string userId,string confirmationToken)
        {
            var result = await _authService.ConfirmUserEmailAsync(userId!, confirmationToken!);
            if (result)
                return RedirectPermanent(@"https://www.google.com/webhp?authuser=0");
            
            return BadRequest(result);
        }
        private string GenerateCallBackUrl(string token, string userId)
        {
            var encodedToken = Uri.EscapeDataString(token);
            var encodedUserId = Uri.EscapeDataString(userId);
            var callBackUrl = $"{Request.Scheme}://{Request.Host}/api/Auth/confirm-email?userId={encodedUserId}&confirmationToken={encodedToken}";
            
            return callBackUrl;
        }

    }
}
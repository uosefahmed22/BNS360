using BNS360.Core.Dtos.Request.Identity;
using BNS360.Core.Errors;
using BNS360.Core.Helpers.Extintions;
using BNS360.Core.Services.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BNS360.Api.Controllers
{
    public class AuthController : ApiBaseController
    {
        private readonly IAuthService _authService;
        //8849b440-6698-441e-924c-7df50359868f
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        //authCon
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterationDto dto)
        {
         
            var result = await _authService.Register(dto, GenerateCallBackUrl);

            if (result.StatusCode == 404)
                return NotFound(result);
            else if (result.StatusCode == 500)
                return StatusCode(500, result);

            return Ok(result);
        }
        
        [HttpPost("forget-password")]
        public async Task<IActionResult> OtpRequest([FromForm] [EmailAddress]string email)
        {
            var result = await _authService.ForgetPassword(email);
            return Ok(result);  
        }

        [HttpPost("verfiy-otp")]
        public IActionResult VerfiyOtp(VerfiyOtp dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Response = new ApiResponse(400), ModelErrors = ModelState.GetModelErrors() });

            var result = _authService.VerfiyOtp(dto);
            return result.StatusCode == 200 ? Ok(result) : BadRequest(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword dto)
        {

            if (!ModelState.IsValid)
                return BadRequest(new { Response = new ApiResponse(400), ModelErrors = ModelState.GetModelErrors() });

            var result = await _authService.ResetPasswordAsync(dto);

            if (result.StatusCode == 500)
                return StatusCode(500, result);
            else if (result.StatusCode == 404)
                return NotFound(result);
            else if (result.StatusCode == 400)
                return BadRequest(result);

            return Ok(result);            
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest dto)
        {

            var result = await _authService.Login(dto);

            if (result.StatusCode != 200)
                return StatusCode(500, result);
            
            return Ok();
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmUserEmail()
        {
            string? userId = Request.Query["userId"];
            string? confirmationToken = Request.Query["confirmationToken"];
            if (userId.IsNullOrEmpty() || confirmationToken.IsNullOrEmpty())
            {
                return BadRequest("invalid userId Or Token");
            }
            var result = await _authService.ConfirmUserEmailAsync(userId!, confirmationToken!);
            if (result)
            {
                return RedirectPermanent(@"https://www.google.com/webhp?authuser=0");
            }
            return BadRequest(result);
        }
        private string GenerateCallBackUrl(string token, string userId)
        {
            // Encode the parameters individually
            var encodedToken = Uri.EscapeDataString(token);
            var encodedUserId = Uri.EscapeDataString(userId);
            // https://www.google.com/
            // Construct the complete URL with encoded parameters
            var callBackUrl = $"{Request.Scheme}://{Request.Host}/api/Auth/confirm-email?userId={encodedUserId}&confirmationToken={encodedToken}";
            
            return callBackUrl;
        }

    }
}
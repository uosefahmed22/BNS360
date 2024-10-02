using BNS360.Core.Dto;
using BNS360.Core.Errors;
using BNS360.Core.IRepository;
using BNS360.Core.Models.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BNS360.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly UserManager<AppUser> _userManager;

        public BusinessController(IBusinessRepository businessRepository,UserManager<AppUser> userManager)
        {
            _businessRepository = businessRepository;
            _userManager = userManager;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "BusinessOwner , Admin")]
        [HttpPost("addbusiness")]
        public async Task<IActionResult> AddBusiness([FromForm] BusinessModelDto model)
        {
            if (ModelState.IsValid)
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (email == null)
                {
                    return BadRequest(new ApiResponse(400, "Invalid user"));
                }
                var user = await _userManager.FindByEmailAsync(email);
                model.userId = user.Id;
                var result = await _businessRepository.CreateBusiness(model);
                if (result.StatusCode == StatusCodes.Status400BadRequest)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            return BadRequest("Invalid model");
        }
        [HttpGet("GetBusinessesByCategoery")]
        public async Task<IActionResult> GetBusinesses(int categoryId)
        {
            var result = await _businessRepository.GetBusinessesByCategoery(categoryId);
            if (result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("getbusiness")]
        public async Task<IActionResult> GetBusiness(int businessId)
        {
            var result = await _businessRepository.GetBusiness(businessId);
            if (result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "BusinessOwner , Admin")]
        [HttpPut("updatebusiness")]
        public async Task<IActionResult> UpdateBusiness(int businessId, [FromForm] BusinessModelDto model)

        {
            if (ModelState.IsValid)
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (email == null)
                {
                    return BadRequest(new ApiResponse(400, "Invalid user"));
                }
                var user = await _userManager.FindByEmailAsync(email);
                model.userId = user.Id;
                var result = await _businessRepository.UpdateBusiness(businessId, model);
                if (result.StatusCode == StatusCodes.Status400BadRequest)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            return BadRequest("Invalid model");
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "BusinessOwner , Admin")]
        [HttpDelete("deletebusiness")]
        public async Task<IActionResult> DeleteBusiness(int businessId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var user = await _userManager.FindByEmailAsync(email);
            var result = await _businessRepository.DeleteBusiness(businessId, user.Id);
            if (result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "BusinessOwner , Admin")]
        [HttpGet("getbusinessesforbusinessowner")]
        public async Task<IActionResult> GetBusinessesForBusinessOwner()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var user = await _userManager.FindByEmailAsync(email);

            var result = await _businessRepository.GetBusinessesForBusinessOwnerAsync(user.Id);
            if (result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("gettopfive")]
        public async Task<IActionResult> GetTopFive()
        {
            var result = await _businessRepository.GetTopFive();
            if (result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

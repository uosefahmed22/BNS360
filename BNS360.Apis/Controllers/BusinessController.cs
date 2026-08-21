using BNS360.Core.Dto;
using BNS360.Core.Errors;
using BNS360.Core.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BNS360.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessRepository _businessRepository;

        public BusinessController(IBusinessRepository businessRepository)
        {
            _businessRepository = businessRepository;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "BusinessOwner , Admin")]
        [HttpPost("addbusiness")]
        public async Task<IActionResult> AddBusiness([FromForm] BusinessModelDto model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (userId == null)
                {
                    return BadRequest(new ApiResponse(400, "Invalid user"));
                }
                model.userId = userId;
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
                var userId = User.FindFirst("UserId")?.Value;
                if (userId == null)
                {
                    return BadRequest(new ApiResponse(400, "Invalid user"));
                }
                model.userId = userId;
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
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var result = await _businessRepository.DeleteBusiness(businessId, userId);
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
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var result = await _businessRepository.GetBusinessesForBusinessOwnerAsync(userId);
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

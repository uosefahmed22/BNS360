using BNS360.Core.Dto;
using BNS360.Core.Errors;
using BNS360.Core.IRepository;
using BNS360.Core.Models;
using BNS360.Core.Models.Auth;
using BNS360.Repository.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Security.Claims;

namespace BNS360.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly UserManager<AppUser> _userManager;

        public PropertyController(IPropertyRepository propertyRepository,UserManager<AppUser> userManager)
        {
            _propertyRepository = propertyRepository;
            _userManager = userManager;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpPost("addProperty")]
        public async Task<IActionResult> AddProperty([FromForm] PropertyModelDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var user = await _userManager.FindByEmailAsync(email);
            model.userId = user.Id;
            var result = await _propertyRepository.AddProperty(model);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpDelete("delete-property")]
        public async Task<IActionResult> DeleteProperty(int propertyId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var user = await _userManager.FindByEmailAsync(email);
            var result = await _propertyRepository.DeleteProperty(propertyId, user.Id);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("get-properties")]
        public async Task<IActionResult> GetProperties()
        {
            var result = await _propertyRepository.GetProperties();
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("get-property")]
        public async Task<IActionResult> GetProperty(int propertyId)
        {
            var result = await _propertyRepository.GetProperty(propertyId);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpPut("update-property")]
        public async Task<IActionResult> UpdateProperty([FromForm] PropertyModelDto model, int PropertyId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var user = await _userManager.FindByEmailAsync(email);
            model.userId = user.Id;
            var result = await _propertyRepository.UpdateProperty(PropertyId, model);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

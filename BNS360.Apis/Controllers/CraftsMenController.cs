using BNS360.Core.Dto;
using BNS360.Core.Errors;
using BNS360.Core.IRepository;
using BNS360.Core.Models;
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
    public class CraftsMenController : ControllerBase
    {
        private readonly ICraftsMenRepository _craftsMenRepository;
        private readonly UserManager<AppUser> _userManager;

        public CraftsMenController(ICraftsMenRepository craftsMenRepository,UserManager<AppUser> userManager)
        {
            _craftsMenRepository = craftsMenRepository;
            _userManager = userManager;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Craftsman")]
        [HttpPost("CreatecraftsMen")]
        public async Task<IActionResult> Create([FromForm] CraftsMenModelDto model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var user = await _userManager.FindByEmailAsync(email);
            model.UserId = user.Id;
            var response = await _craftsMenRepository.Create(model);
            return Ok(response);
        }
        [HttpGet("GetAllCraftsMen")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _craftsMenRepository.GetAll();
            return Ok(response);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Craftsman")]
        [HttpGet("getcraftsmenbyuserid")]
        public async Task<IActionResult> GetById()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var user = await _userManager.FindByEmailAsync(email);
            var response = await _craftsMenRepository.GetByUserId(user.Id);
            return Ok(response);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Craftsman")]
        [HttpPut("UpdateCraftsMen")]
        public async Task<IActionResult> Update(int CraftsMenId,[FromForm] CraftsMenModelDto model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var user = await _userManager.FindByEmailAsync(email);
            model.UserId = user.Id;
            var response = await _craftsMenRepository.Update(CraftsMenId,model);
            return Ok(response);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Craftsman")]
        [HttpDelete("DeleteCraftsMen")]
        public async Task<IActionResult> Delete(int CraftsMenId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var user = await _userManager.FindByEmailAsync(email);

            var response = await _craftsMenRepository.Delete(CraftsMenId, user.Id);
            return Ok(response);
        }
    }
}

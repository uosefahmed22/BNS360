using BNS360.Core.Errors;
using BNS360.Core.IRepository;
using BNS360.Core.Models.Auth;
using BNS360.Repository.Repository;
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
    public class SavedJobsController : ControllerBase
    {
        private readonly ISavedJobsRepository _savedJobsRepository;
        private readonly UserManager<AppUser> _userManager;

        public SavedJobsController(ISavedJobsRepository savedJobsRepository,UserManager<AppUser> userManager)
        {
            _savedJobsRepository = savedJobsRepository;
            _userManager = userManager;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpGet("get-saved-jobs")]
        public async Task<IActionResult> GetSavedJobs()
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
            var result = await _savedJobsRepository.GetSavedJobs(user.Id);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpPost("save-job")]
        public async Task<IActionResult> SaveJob(int jobId)
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
            var result = await _savedJobsRepository.SaveJob(jobId, user.Id);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpDelete("unsave-job")]
        public async Task<IActionResult> UnSaveJob(int jobId)
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
            var result = await _savedJobsRepository.UnSaveJob(jobId, user.Id);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

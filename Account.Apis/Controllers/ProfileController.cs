using Account.Apis.Errors;
using Account.Core.Dtos.Account;
using Account.Reposatory.Services.Authentications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Account.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService progfileService)
        {
            _profileService = progfileService;
        }
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var response = await _profileService.DeleteUserAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(string userId)
        {
            var user = await _profileService.GetProfileAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiResponse(404, "User not found."));
            }
            return Ok(user);
        }

        [HttpPatch("updateImage")]
        public async Task<IActionResult> UpdateUserImage([FromForm] UpdateUserImageModel model)
        {
            var response = await _profileService.UpdateUserImageAsync(model);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("{userId}/name")]
        public async Task<IActionResult> UpdateUserName(string userId, [FromBody] string newName)
        {
            var response = await _profileService.UpdateUserNameAsync(userId, newName);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("jobs/{userId}")]
        public async Task<IActionResult> GetMyPostsInJobs(string userId)
        {
            try
            {
                var jobs = await _profileService.GetMyPostsInJobs(userId);
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to retrieve jobs: {ex.Message}");
            }
        }

        [HttpGet("properties/{userId}")]
        public async Task<IActionResult> GetMyPostsInProperties(string userId)
        {
            try
            {
                var properties = await _profileService.GetMyPostsInProperties(userId);
                return Ok(properties);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to retrieve properties: {ex.Message}");
            }
        }

        [HttpPatch("changeRole")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _profileService.ChangeUserRoleAsync(dto);
            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            return Ok(result.Message);
        }
    }
}

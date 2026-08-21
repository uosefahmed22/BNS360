using BNS360.Core.Dto;
using BNS360.Core.Errors;
using BNS360.Core.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BNS360.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobRepository _jobRepository;

        public JobController(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpPost("AddJob")]
        public async Task<IActionResult> AddJob(JobModelDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            model.UserId = userId;
            var result = await _jobRepository.AddJob(model);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpPut("UpdateJob")]
        public async Task<IActionResult> UpdateJob(int JobId, JobModelDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            model.UserId = userId;
            var result = await _jobRepository.UpdateJob(JobId, userId, model);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpDelete("DeleteJob")]
        public async Task<IActionResult> DeleteJob(int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var result = await _jobRepository.DeleteJob(JobId, userId);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("GetJobById")]
        public async Task<IActionResult> GetJobById(int JobId)
        {
            var result = await _jobRepository.GetJobById(JobId);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("GetAllJobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            var result = await _jobRepository.GetAllJobs();
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

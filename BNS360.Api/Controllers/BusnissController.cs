using BNS360.Core.Abstractions;
using BNS360.Core.Dtos.Request;
using BNS360.Core.Entities;
using BNS360.Core.Helpers.Enums;
using BNS360.Core.Services.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BNS360.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusnissController : ControllerBase
    {
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IBusnissRepository _busnissRepository;
        public BusnissController(IGenericRepository<Category> categoryRepo, IBusnissRepository busnissRepository, IGenericRepository<Busniss> busnissRepo)
        {
            _categoryRepo = categoryRepo;
            _busnissRepository = busnissRepository;
        }
        [HttpPost("createBusniss")]
        [Authorize(Roles = nameof(UserType.BusinssOwner))]

        public async Task<IActionResult> CreateBusniss([FromBody] BusnisRequest busniss)
        {
            var userId = HttpContext.User.FindFirst("userId")?.Value;
            if (userId is null)
                return Unauthorized();
        
            var result = await _busnissRepository.CreateAsync(busniss,Guid.Parse(userId));
            if (result.StatusCode == 500)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,result.Message);
            }
            return Ok(result);
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetBusnissById(Guid id)
        {  
            return Ok(await _busnissRepository.GetByIdAsync(id));
        }
        [HttpGet("getByCategoryId")]
        public async Task<IActionResult> GetCategoryBusniss(Guid Id)
        {
            var result = await _busnissRepository.GetAllBusnissWithCategoryIdAsync(Id);
            return Ok(result);
        }
        
        [HttpGet("getAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            return Ok(await _categoryRepo.GetAllAsync());   
        }
        [HttpGet("getRecommended")]
        public async Task<IActionResult> GetRecommended()
        {
            var result = await _busnissRepository.GetRecommendedAsync();
            return Ok(result);  
        }
    }
}

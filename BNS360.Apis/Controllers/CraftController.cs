using BNS360.Core.Dto;
using BNS360.Core.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BNS360.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CraftController : ControllerBase
    {
        private readonly ICraftRepository _craftRepository;

        public CraftController(ICraftRepository craftRepository)
        {
            _craftRepository = craftRepository;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost]
        [Route("createCraft")]
        public async Task<IActionResult> CreateCraft([FromForm] CraftsModelDto model)
        {
            var response = await _craftRepository.CreateCraft(model);
            return Ok(response);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete]
        [Route("deleteCraft")]
        public async Task<IActionResult> DeleteCraft(int craftId)
        {
            var response = await _craftRepository.DeleteCraft(craftId);
            return Ok(response);
        }
        [HttpGet("getCrafts")]
        public async Task<IActionResult> GetCrafts()
        {
            var response = await _craftRepository.GetCrafts();
            return Ok(response);
        }
        [HttpGet("getCraftById")]
        public async Task<IActionResult> GetCraftById(int craftId)
        {
            var response = await _craftRepository.GetCraft(craftId);
            return Ok(response);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut("updateCraft")]
        public async Task<IActionResult> UpdateCraft(int craftId, [FromForm] CraftsModelDto model)
        {
            var response = await _craftRepository.UpdateCraft(craftId, model);
            return Ok(response);
        }
    }
}

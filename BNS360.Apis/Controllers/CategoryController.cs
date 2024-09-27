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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost("addcategory")]
        public async Task<IActionResult> AddCategory(CategoryModelDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryRepository.CreateCategory(model);
                if (result.StatusCode == StatusCodes.Status400BadRequest)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            return BadRequest("Invalid model");
        }
        [HttpGet("getcategories")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _categoryRepository.GetCategories();
            if (result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("getcategory")]
        public async Task<IActionResult> GetCategory(int categoryId)
        {
            var result = await _categoryRepository.GetCategory(categoryId);
            if (result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut("updatecategory")]
        public async Task<IActionResult> UpdateCategory(int categoryId, CategoryModelDto model)

        {
            if (ModelState.IsValid)
            {
                var result = await _categoryRepository.UpdateCategory(categoryId, model);
                if (result.StatusCode == StatusCodes.Status400BadRequest)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            return BadRequest("Invalid model");
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("deletecategory")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var result = await _categoryRepository.DeleteCategory(categoryId);
            if (result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

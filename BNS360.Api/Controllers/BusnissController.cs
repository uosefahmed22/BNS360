using BNS360.Core.Abstractions;
using BNS360.Core.Dtos.Request;
using BNS360.Core.Dtos.Response;
using BNS360.Core.Dtos.Response.AppBusniss;
using BNS360.Core.Entities;
using BNS360.Core.Errors;
using BNS360.Core.Helpers;
using BNS360.Core.Helpers.Enums;
using BNS360.Core.Services.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BNS360.Api.Controllers;

public class BusnissController(IGenericRepository<Category> categoryRepo, IBusnissRepository busnissRepository) : ApiBaseController
{
    private readonly IGenericRepository<Category> _categoryRepo = categoryRepo;
    private readonly IBusnissRepository _busnissRepository = busnissRepository;

    

    #region PostEndpoints
    [Authorize(Roles = nameof(UserType.BusinssOwner))]
    [HttpPost("create")]
    public async Task<IActionResult> CreateBusniss([FromBody] BusnisRequest busniss)
    {
        var userId = HttpContext.User.FindFirst("userId")?.Value;
        if (userId is null)
            return Unauthorized(new ApiResponse(401,"InValid User ID"));
    
        var result = await _busnissRepository.CreateAsync(busniss,Guid.Parse(userId));

        if (result.StatusCode == 409)
            return Conflict(result);
        else if(result.StatusCode == 400)
            return BadRequest(result);
        
        return Ok(result);
    }

    [Authorize(Roles = nameof(UserType.BusinssOwner))]
    [HttpPost("{id}/addProfilePicture")]
    public async Task<IActionResult> AddProfilePictureAsync([FromForm]IFormFile picture,Guid id)
    {
        var ownerId = HttpContext.User.FindFirst("userId")?.Value;

        if (ownerId is null)
            return Unauthorized();

        var result = await _busnissRepository.SetProfilePictureAsync(picture, id,Guid.Parse(ownerId));
        if (result.StatusCode != 500)
        {
            return StatusCode(result.StatusCode, result);
        }       
        return Ok(result);
    }
    [Authorize(Roles = nameof(UserType.BusinssOwner))]
    [HttpPost("{id}/addToAlbum")]
    public async Task<IActionResult> AddToAlbumAsync(
        [ListSize(maxSize:5,minSize:1)] List<IFormFile> album,
        Guid id)
    {
        var ownerId = HttpContext.User.FindFirst("userId")?.Value;

        if (ownerId is null)
            return Unauthorized();

        var result = await _busnissRepository.AddBusnissAlbumAsync(album, id, Guid.Parse(ownerId));

        return result.StatusCode == 200 ? Ok(result) : StatusCode(result.StatusCode,result); 
    }


    [Authorize]
    [HttpPost("addReview")]
    public async Task<IActionResult> AddReview(ReviewRequest review)
    {
        
        var userId = HttpContext.User.FindFirst("userId")?.Value;
        if (userId is null)
            return Unauthorized(new ApiResponse(401, "InValid User ID"));

        var result = await _busnissRepository.AddReviewAsync(review,Guid.Parse(userId));
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpPost("addToFavorites/{id}")]
    public async Task<IActionResult> AddToFavorites(Guid id)
    {
        var userId = HttpContext.User.FindFirst("userId")?.Value;
        if (userId is null)
            return Unauthorized(new ApiResponse(401, "InValid User ID"));

        var result = await _busnissRepository.AddToFavorites(id, Guid.Parse(userId));

        return StatusCode(result.StatusCode, result);
    }
    #endregion

    #region GetEndPoints
    [HttpGet("getById")]
    public async Task<IActionResult> GetBusnissById(Guid id)
    {  
        var result = await _busnissRepository.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);   
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
    [HttpGet("search")]
    public async Task<IActionResult> SearchAsync([FromQuery] string key, [FromQuery] Guid categoryId)
    {
        var result = await _busnissRepository.SearchAsync(key, categoryId);
        return result is null || !result.Any() ? NotFound(result) : Ok(result);   
    }
    [HttpGet("getRecommended")]
    public async Task<IActionResult> GetRecommended()
    {
        var result = await _busnissRepository.GetRecommendedAsync();
        return Ok(result);  
    }
    [HttpGet("getPage")]
    public async Task<IActionResult> GetPage(
        [FromQuery] int pageNumber,
        [FromQuery] int size,
        [FromQuery] Guid categoryId)
    {
        var result = await _busnissRepository.Paganate(pageNumber, size,categoryId);
        return result is null ||
               (result is PagenationResponse<BusnissReponse> response && !response.Items.Any()) ?
               NotFound() : StatusCode(result.StatusCode, result);

    }

    [Authorize]
    [HttpGet("getFavorites")]
    public async Task<IActionResult> GetFavorites()
    {
        var userId = HttpContext.User.FindFirst("userId")?.Value;
        if (userId is null)
            return Unauthorized(new ApiResponse(401, "InValid User ID"));

        var result = await _busnissRepository.GetFavorites(Guid.Parse(userId));

        return Ok(result);

    }

    [HttpPost("filterByDistance")]
    public async Task<IActionResult> FilterByDistanceAsync(
        [FromQuery] Guid categoryId,
        [FromQuery] int pageNumber,
        [FromQuery] int size,
        [FromBody] CurrentLocation currentLocation)
    {
        var result = await _busnissRepository.FilterByDistance(categoryId,currentLocation, pageNumber, size);
        
        return result is null ||
                (result is PagenationResponse<BusnissReponse> response && !response.Items.Any()) ?
                NotFound() : StatusCode(result.StatusCode, result);

    }
    [HttpGet("filter")]
    public async Task<IActionResult> FilterAsync(
        [FromQuery] Guid categoryId,
        [FromQuery] int pageNumber,
        [FromQuery] int size,
    [FromQuery,Required,EnumDataType(typeof(FilterType),ErrorMessage = "Only Rating(1) And (Active)2")]
    FilterType filterType)
    {
        var result = filterType switch
        {

            FilterType.Rating => await _busnissRepository.FilterByRatings(categoryId,pageNumber, size),
            FilterType.Active => await _busnissRepository.FilterByActive(categoryId,pageNumber, size),
            _ => null
        };
        return result is null || 
                (result is PagenationResponse<BusnissReponse> response && !response.Items.Any())?
                NotFound("No Busniss Was Found") : StatusCode(result.StatusCode, result);
    }


    [HttpGet("{id}/getReviews")]
    public async Task<IActionResult> GetReviewsAsync([FromQuery]int pageNumber, [FromQuery]int size, Guid id)
    {
        var result = await _busnissRepository.GetReviewsAsync(id, pageNumber, size);
        return result is null ? NotFound() : Ok(result);    
    }
    #endregion

    #region UpdateEndPoints
    [Authorize(Roles = nameof(UserType.BusinssOwner))]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateAsync(BusnisUpdateRequest busniss)
    {
        var ownerId = HttpContext.User.FindFirst("userId")?.Value;

        if (ownerId is null)
            return Unauthorized(new ApiResponse(401, "InValid User ID"));

        var result = await _busnissRepository.UpdateAsync(busniss,Guid.Parse(ownerId));
        if (result.StatusCode is 401)
        {
            return Unauthorized(result);
        }
        return NoContent();
    }

    [Authorize]
    [HttpPut("{id}/updateReview")]
    public async Task<IActionResult> UpdateReviewAsync(Guid id ,ReviewRequest review,Guid reviewId)
    {

        var userId = HttpContext.User.FindFirst("userId")?.Value;
        if (userId is null)
            return Unauthorized(new ApiResponse(401, "InValid User ID"));

        var result = await _busnissRepository.UpdateReviewAsync(id,review,reviewId, Guid.Parse(userId));
        return StatusCode(result.StatusCode, result);
    }


    #endregion

    #region DeleteEndPoints
    [Authorize(Roles = nameof(UserType.BusinssOwner))]
    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> RemoveAsync(Guid id)
    {
        var ownerId = HttpContext.User.FindFirst("userId")?.Value;

        if (ownerId is null)
            return Unauthorized(new ApiResponse(401, "InValid User ID"));

        var result = await _busnissRepository.DeleteAsync(id,Guid.Parse(ownerId));

        if (result.StatusCode == 401)
            return Unauthorized(result);

        return NoContent();
    }
    [Authorize]
    [HttpDelete("removeFromFavorites/{id}")]
    public async Task<IActionResult> removeFromFavorites(Guid id)
    {
        var userId = HttpContext.User.FindFirst("userId")?.Value;
        if (userId is null)
            return Unauthorized(new ApiResponse(401, "InValid User ID"));

        var result = await _busnissRepository.RemoveFromFavorites(id, Guid.Parse(userId));

        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = nameof(UserType.BusinssOwner))]
    [HttpDelete("{id}/removeFromAlbum")]
    public async Task<IActionResult> RemoveFromAlbumAsync(Guid id, [FromQuery] string url)
    {
        var ownerId = HttpContext.User.FindFirst("userId")?.Value;
        if (ownerId is null)
            return Unauthorized();

        var result = await _busnissRepository.RemoveFromAlbumAsync(url, id, Guid.Parse(ownerId));
        return StatusCode(result.StatusCode, result);
    }
    [Authorize, HttpDelete("{busnissId}/removeReview")]
    public async Task<IActionResult> RemoveReviewAsync(Guid busnissId,Guid reviewId)
    {
        
        var userId = HttpContext.User.FindFirst("userId")?.Value;
        if (userId is null)
            return Unauthorized();
        var result = await _busnissRepository.RemoveReviewAsync(busnissId, reviewId, Guid.Parse(userId));
        return StatusCode(result.StatusCode, result);
    }
    #endregion
}

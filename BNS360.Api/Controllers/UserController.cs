using BNS360.Core.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BNS360.Api.Controllers;
public class UserController : ApiBaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpPost("setProfilePicture")]
    public async Task<IActionResult> SetProfilePictureAsync([Required]IFormFile picture)
    {
        var userId = HttpContext.User.FindFirst("userId")?.Value;
        if (userId is null)
        {
            return Unauthorized();  
        }
        var result = await _userService.SetProfilePictureAsync(picture, userId);

        return StatusCode(result.StatusCode,result);
    }        
    [Authorize]
    [HttpDelete("reomveProfilePicture")]
    public async Task<IActionResult> DeleteProfilePictureAsync()
    {
        var userId = HttpContext.User.FindFirst("userId")?.Value;
        if (userId is null)
        {
            return Unauthorized();  
        }
        var result = await _userService.DeleteProfilePictureAsync(userId);

        return StatusCode(result.StatusCode,result);
    }

}

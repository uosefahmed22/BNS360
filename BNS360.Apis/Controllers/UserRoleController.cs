using BNS360.Core.Errors;
using BNS360.Core.IServices.Auth;
using BNS360.Core.Models.Auth;
using BNS360.Repository.Data;
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
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

    public class UserRoleController : ControllerBase
    {
        private readonly AppDbContext _dbcontext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRoleService _userRoleService;

        public UserRoleController(AppDbContext dbcontext
            , UserManager<AppUser> userManager
            , RoleManager<IdentityRole> roleManager,
            IUserRoleService userRoleService
            )
        {
            _dbcontext = dbcontext;
            _userManager = userManager;
            _roleManager = roleManager;
            _userRoleService = userRoleService;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("getRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _userRoleService.GetRolesAsync();
            return Ok(roles);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost("createRole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var role = await _userRoleService.CreateRole(roleName);
            return Ok(role);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("deleteRole")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var role = await _userRoleService.DeleteRole(roleName);
            return Ok(role);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost("addUserToRole")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            var result = await _userRoleService.AddUserToRole(email, roleName);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost("removeUserFromRole")]
        public async Task<IActionResult> RemoveUserFromRole(string email, string roleName)
        {
            var result = await _userRoleService.RemoveUserFromRole(email, roleName);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRoleService.GetUsers();
            return Ok(users);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("getRolesByUser")]
        public async Task<IActionResult> GetRolesByUser(string email)
        {
            var roles = await _userRoleService.GetRolesByUser(email);
            return Ok(roles);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpPatch("addProfileImage")]
        public async Task<IActionResult> AddProfileImage(IFormFile image)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await _userRoleService.AddProfileImage(image, null, email);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpGet("getUser")]
        public async Task<IActionResult> GetUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await _userRoleService.GetUser(email);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User , Admin")]
        [HttpDelete("deleteUser")]
        public async Task<IActionResult> DeleteUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var result = await _userRoleService.DeleteUser(email);
            return Ok(result);
        }
    }
}

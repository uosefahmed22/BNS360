using BNS360.Core.CustemExceptions;
using BNS360.Core.Entities.Identity;
using BNS360.Core.Errors;
using BNS360.Core.Services.Authentication;
using BNS360.Core.Services.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Reposatory.Repositories.Authentication
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileService _fileService;

        public UserService(UserManager<AppUser> userManager, IFileService fileService)
        {
            _userManager = userManager;
            _fileService = fileService;
        }

        public async Task<ApiResponse> DeleteProfilePictureAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);    
            if (user is null)
            {
                return new ApiResponse(StatusCodes.Status404NotFound);
            }

            if (user.profilePictureUrl is not null 
                && File.Exists(user.profilePictureUrl))
            {
                File.Delete(user.profilePictureUrl);
            }
            else
            {
                return new ApiResponse(StatusCodes.Status404NotFound, "No Profile Picture Was Found To Delete");  
            }

            return new ApiResponse(StatusCodes.Status204NoContent);
        }

        public async Task<(string username, string? profilePictureUrl)> GetMainProfileIfoAsync(Guid userId)
        {
             var user = await _userManager.FindByIdAsync(userId.ToString());
            
             if (user is null)
                throw new ItemNotFoundException("User Not Found");

             return (user.Name, user.profilePictureUrl);
        }

        public async Task<ApiResponse> SetProfilePictureAsync(IFormFile picture, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return new ApiResponse(StatusCodes.Status404NotFound);

            if (user.profilePictureUrl is not null
                && File.Exists(user.profilePictureUrl))
            {
                File.Delete(user.profilePictureUrl);
            }

            var fileName = user.Id.ToString().Substring(0,8) + Path.GetRandomFileName();

            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","Users");

            Directory.CreateDirectory(uploadDirectory);

            user.profilePictureUrl = await _fileService.StoreAsync(picture, uploadDirectory, fileName);

            await _userManager.UpdateAsync(user);

            return new ApiResponse(StatusCodes.Status200OK);
        }
    }
}

using BNS360.Core.Errors;
using Microsoft.AspNetCore.Http;

namespace BNS360.Core.Services.Authentication;

public interface IUserService
{
    Task<(string username, string? profilePictureUrl)> GetMainProfileIfoAsync(Guid userId); 
    Task<ApiResponse> SetProfilePictureAsync(IFormFile picture,string userId); 
    Task<ApiResponse> DeleteProfilePictureAsync(string userId);   
}

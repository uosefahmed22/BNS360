using BNS360.Core.Errors;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.IServices
{
    public interface IProfileService
    {
        Task<ApiResponse> UpdateUserImageAsync(string userId, IFormFile image);
        Task<ApiResponse> GetUser(string email);
        Task<ApiResponse> DeleteUser(string email);
        Task<ApiResponse> UpdateUserFullName(string fullName, string email);
        Task<ApiResponse> GetUserBusiness(string UserId);
        Task<ApiResponse> GetUserJobs(string UserId);
        Task<ApiResponse> GetUserCraftsMen(string UserId);
    }
}

using BNS360.Core.Errors;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.IServices.Auth
{
    public interface IUserRoleService
    {
        Task<ApiResponse> GetRolesAsync();
        Task<ApiResponse> CreateRole(string roleName);
        Task<ApiResponse> DeleteRole(string roleName);
        Task<ApiResponse> AddUserToRole(string email, string roleName);
        Task<ApiResponse> RemoveUserFromRole(string email, string roleName);
        Task<ApiResponse> GetUsers();
        Task<ApiResponse> GetRolesByUser(string email);
        Task<ApiResponse> AddProfileImage(IFormFile? image, string? ImageUrl, string email);
        Task<ApiResponse> GetUser(string email);
        Task<ApiResponse> DeleteUser(string email);
    }
}

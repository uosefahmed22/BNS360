using BNS360.Core.Dto;
using BNS360.Core.Errors;
using BNS360.Core.IServices;
using BNS360.Core.Models.Auth;
using BNS360.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Repository.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _dbContext;
        private readonly IImageService _imageService;

        public ProfileService(UserManager<AppUser> userManager, AppDbContext dbContext, IImageService imageService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _imageService = imageService;
        }

        public async Task<ApiResponse> DeleteUser(string email)
        {
            try
            {
                var user = _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new ApiResponse(404, "المستخدم غير موجود");
                }
                var result = await _userManager.DeleteAsync(user.Result);
                if (result.Succeeded)
                {
                    return new ApiResponse(200, "تم حذف المستخدم بنجاح");
                }
                return new ApiResponse(400, "فشل في حذف المستخدم");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> GetUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ApiResponse(404, "المستخدم غير موجود");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto
            {
                Email = user.Email,
                FullName = user.FullName,
                ImageUrl = user.ImageUrl,
                UserRole = (List<string>)roles
            };

            return new ApiResponse(200, userDto);
        }
        public async Task<ApiResponse> UpdateUserImageAsync(string userId, IFormFile image)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponse(404, "User not found.");
            }

            if (image == null)
            {
                if (!string.IsNullOrEmpty(user.ImageUrl))
                {
                    await _imageService.DeleteImageAsync(user.ImageUrl);
                    user.ImageUrl = null;
                }
            }
            else
            {
                var saveResult = await _imageService.UploadImageAsync(image);
                if (saveResult.Item1 == 0)
                {
                    return new ApiResponse(400, saveResult.Item2);
                }

                if (!string.IsNullOrEmpty(user.ImageUrl))
                {
                    await _imageService.DeleteImageAsync(user.ImageUrl);
                }

                user.ImageUrl = saveResult.Item2;
            }
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return new ApiResponse(500, "Failed to update user image.");
            }

            return new ApiResponse(200, "User image updated successfully.");
        }
        public async Task<ApiResponse> UpdateUserFullName(string fullName, string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new ApiResponse(404, "المستخدم غير موجود");
                }
                user.FullName = fullName;
                await _userManager.UpdateAsync(user);
                return new ApiResponse(200, "تم تعديل الاسم بنجاح");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> GetUserBusiness(string UserId) { 
            try
            {
                var business = await _dbContext
                    .BusinessModels
                    .Where(x => x.UserId == UserId)
                    .Select(x => new
                    {
                        x.Id,
                        x.BusinessNameArabic,
                        x.BusinessNameEnglish,
                        x.BusinessDescriptionArabic,
                        x.BusinessDescriptionEnglish,
                        x.BusinessAddressArabic,
                        x.BusinessAddressEnglish,
                        x.Numbers,
                        x.ImageUrls,
                        x.Latitude,
                        x.Longitude,
                        x.Opening,
                        x.Closing,
                        x.CategoriesModelId,
                        x.Holidays,
                        x.ProfileImageUrl
                    })
                    .ToListAsync();
                if (business == null)
                {
                    return new ApiResponse(404, "المستخدم ليس لديه عمل");
                }
                return new ApiResponse(200, business);
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> GetUserJobs(string UserId)
        {
            try
            {
                var jobs = await _dbContext
                    .Jobs
                    .Where(x => x.UserId == UserId)
                    .Select(x => new
                    {
                        x.Id,
                        x.JobTitleArabic,
                        x.JobTitleEnglish,
                        x.JobDescriptionArabic,
                        x.JobDescriptionEnglish,
                        x.AddreesInArabic,
                        x.AddreesInEnglish,
                        x.Numbers,
                        x.Type,
                        x.WorkHours,
                        x.Salary,
                        x.Requirements,
                        x.TimeAddedjob
                    })
                    .ToListAsync();
                if (jobs == null)
                {
                    return new ApiResponse(404, "المستخدم ليس لديه وظائف");
                }
                return new ApiResponse(200, jobs);
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> GetUserCraftsMen(string UserId) { 
            try
            {
                var craftsMen = await _dbContext
                    .CraftsMen
                    .Where(x => x.UserId == UserId)
                    .Select(x => new
                    {
                        x.Id,
                        x.CraftsMenNameArabic,
                        x.CraftsMenNameEnglish,
                        x.CraftsMenDescriptionArabic,
                        x.CraftsMenDescriptionEnglish,
                        x.CraftsMenAddressArabic,
                        x.CraftsMenAddressEnglish,
                        x.Holidays,
                        x.Numbers,
                        x.Opening,
                        x.Closing,
                        x.ProfileImageUrl,
                        x.ImageUrls,
                        x.CraftsModelId
                    }).ToListAsync();
                if (craftsMen == null)
                {
                    return new ApiResponse(404, "المستخدم ليس لديه حرفيين");
                }
                return new ApiResponse(200, craftsMen);
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
    }
}

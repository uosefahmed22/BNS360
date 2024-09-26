using AutoMapper;
using BNS360.Core.Dto;
using BNS360.Core.Errors;
using BNS360.Core.IRepository;
using BNS360.Core.Models;
using BNS360.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Repository.Repository
{
    public class FavoriteBusinessRepository : IFavoriteBusinessRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public FavoriteBusinessRepository(AppDbContext dbContext,IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<ApiResponse> AddBusinessToFavorite(string userId, int businessId)
        {
            try
            {
                var ExistingFavorite = await _dbContext
                    .Favorites
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.businessId == businessId);
                if (ExistingFavorite != null)
                    return new ApiResponse(400, "هذا العنصر موجود بالفعل في المفضلة");
                var favorite = new FavoriteModel
                {
                    UserId = userId,
                    businessId = businessId
                };
                await _dbContext.Favorites.AddAsync(favorite);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تمت الاضافة بنجاح");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> GetFavoriteBusinesses(string userId)
        {
            try
            {
                var favorites = await _dbContext
                .Favorites
                .Where(x => x.UserId == userId)
                .Include(x => x.BusinessModel)
                .Select(x => new
                {
                    x.businessId,
                    x.BusinessModel.BusinessNameArabic,
                    x.BusinessModel.BusinessNameEnglish,
                    x.BusinessModel.BusinessDescriptionArabic,
                    x.BusinessModel.BusinessDescriptionEnglish,
                    x.BusinessModel.BusinessAddressArabic,
                    x.BusinessModel.BusinessAddressEnglish,
                    x.BusinessModel.Numbers,
                    x.BusinessModel.ImageUrls,
                    x.BusinessModel.Latitude,
                    x.BusinessModel.Longitude,
                    x.BusinessModel.Opening,
                    x.BusinessModel.Closing,
                    x.BusinessModel.CategoriesModelId,
                    x.BusinessModel.Holidays,
                    x.BusinessModel.ProfileImageUrl
                }).ToListAsync();
                return new ApiResponse(200, favorites);
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> RemoveBusinessFromFavorite(string userId, int businessId)
        {
            try
            {
                var ExistingFavorite = await _dbContext
                    .Favorites
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.businessId == businessId);
                if (ExistingFavorite == null)
                    return new ApiResponse(400, "هذا العنصر غير موجود في المفضلة");
                _dbContext.Favorites.Remove(ExistingFavorite);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تمت الازالة بنجاح");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }

        public async Task<ApiResponse> AddCraftsMenToFavorite(string userId, int businessId)
        {
            try
            {
                var ExistingFavorite = await _dbContext
                    .Favorites
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.CraftsMenId == businessId);
                if (ExistingFavorite != null)
                    return new ApiResponse(400, "هذا العنصر موجود بالفعل في المفضلة");
                var favorite = new FavoriteModel
                {
                    UserId = userId,
                    CraftsMenId = businessId
                };
                await _dbContext.Favorites.AddAsync(favorite);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تمت الاضافة بنجاح");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> GetCraftsMenFavorites(string userId)
        {
            try
            {
                var favorites = await _dbContext
                .Favorites
                .Where(x => x.UserId == userId)
                .Include(x => x.CraftsMenModel)
                .Select(x => new
                {
                    x.CraftsMenId,
                    x.CraftsMenModel.CraftsMenNameArabic,
                    x.CraftsMenModel.CraftsMenNameEnglish,
                    x.CraftsMenModel.CraftsMenDescriptionArabic,
                    x.CraftsMenModel.CraftsMenDescriptionEnglish,
                    x.CraftsMenModel.CraftsMenAddressArabic,
                    x.CraftsMenModel.CraftsMenAddressEnglish,
                    x.CraftsMenModel.Numbers,
                    x.CraftsMenModel.ImageUrls,
                    x.CraftsMenModel.Opening,
                    x.CraftsMenModel.Closing,
                    x.CraftsMenModel.CraftsModelId,
                    x.CraftsMenModel.Holidays,
                    x.CraftsMenModel.ProfileImageUrl
                }).ToListAsync();
                return new ApiResponse(200, favorites);
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> RemovecraftsMenFromFavorite(string userId, int craftsMenId)
        {
            try
            {
                var ExistingFavorite = await _dbContext
                    .Favorites
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.CraftsMenId == craftsMenId);
                if (ExistingFavorite == null)
                    return new ApiResponse(400, "هذا العنصر غير موجود في المفضلة");
                _dbContext.Favorites.Remove(ExistingFavorite);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تمت الازالة بنجاح");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
    }
}

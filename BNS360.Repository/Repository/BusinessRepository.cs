using AutoMapper;
using BNS360.Core.Dto;
using BNS360.Core.Errors;
using BNS360.Core.IRepository;
using BNS360.Core.IServices;
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
    public class BusinessRepository : IBusinessRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public BusinessRepository(AppDbContext context, IMapper mapper, IImageService fileService)
        {
            _dbContext = context;
            _mapper = mapper;
            _imageService = fileService;
        }
        public async Task<ApiResponse> CreateBusiness(BusinessModelDto model)
        {
            try
            {
                var ExsistingBusiness = await _dbContext
                    .BusinessModels
                    .FirstOrDefaultAsync(x => x.BusinessNameArabic == model.BusinessNameArabic);
                if (ExsistingBusiness != null)
                {
                    return new ApiResponse(400, "البزنس موجود مسبقا");
                }
                if (model.ProfileImage != null)
                {
                    var fileResult = await _imageService.UploadImageAsync(model.ProfileImage);
                    if (fileResult.Item1 == 1)
                    {
                        model.ProfileImageUrl = fileResult.Item2;
                    }
                    else
                    {
                        return new ApiResponse(400, fileResult.Item2);
                    }
                }
                if (model.Images != null)
                {
                    if (model.ImageUrls == null)
                    {
                        model.ImageUrls = new List<string>();
                    }

                    foreach (var image in model.Images)
                    {
                        var fileResult = await _imageService.UploadImageAsync(image);
                        if (fileResult.Item1 == 1)
                        {
                            model.ImageUrls.Add(fileResult.Item2);
                        }
                        else
                        {
                            return new ApiResponse(400, fileResult.Item2);
                        }
                    }
                }
                var business = _mapper.Map<BusinessModelDto, BusinessModel>(model);
                await _dbContext.BusinessModels.AddAsync(business);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تم اضافة البزنس بنجاح");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> DeleteBusiness(int businessId, string Userid)
        {
            var business = await _dbContext.BusinessModels
                .Where(x => x.UserId == Userid)
                .FirstOrDefaultAsync(x => x.Id == businessId);
            if (business == null)
            {
                return new ApiResponse(404, "البزنس غير موجود");
            }
            if (!string.IsNullOrEmpty(business.ProfileImageUrl))
            {
                await _imageService.DeleteImageAsync(business.ProfileImageUrl);
            }
            if (business.ImageUrls != null)
            {
                foreach (var imageUrl in business.ImageUrls)
                {
                    await _imageService.DeleteImageAsync(imageUrl);
                }
            }
            _dbContext.BusinessModels.Remove(business);
            await _dbContext.SaveChangesAsync();
            return new ApiResponse(200, "تم حذف البزنس بنجاح");
        }
        public async Task<ApiResponse> GetBusiness(int businessId)
        {
            var business = await _dbContext
                .BusinessModels
                .Select(x => new BusinessModel
                {
                    Id = x.Id,
                    BusinessNameArabic = x.BusinessNameArabic,
                    BusinessNameEnglish = x.BusinessNameEnglish,
                    BusinessDescriptionArabic = x.BusinessDescriptionArabic,
                    BusinessDescriptionEnglish = x.BusinessDescriptionEnglish,
                    BusinessAddressArabic = x.BusinessAddressArabic,
                    BusinessAddressEnglish = x.BusinessAddressEnglish,
                    Holidays = x.Holidays,
                    Numbers = x.Numbers,
                    Opening = x.Opening,
                    Closing = x.Closing,
                    Longitude = x.Longitude,
                    Latitude = x.Latitude,
                    ProfileImageUrl = x.ProfileImageUrl,
                    ImageUrls = x.ImageUrls,
                    CategoriesModelId = x.CategoriesModelId
                }).FirstOrDefaultAsync(x => x.Id == businessId);
            if (business == null)
            {
                return new ApiResponse(404, "البزنس غير موجود");
            }
            return new ApiResponse(200, business);
        }
        public async Task<ApiResponse> GetBusinessesByCategoery(int categoryId)
        {
            var businesses = await _dbContext
                .BusinessModels
                .Where(x => x.CategoriesModelId == categoryId)
                .Select(x => new
                {
                    Id = x.Id,
                    BusinessNameArabic = x.BusinessNameArabic,
                    BusinessNameEnglish = x.BusinessNameEnglish,
                    BusinessDescriptionArabic = x.BusinessDescriptionArabic,
                    BusinessDescriptionEnglish = x.BusinessDescriptionEnglish,
                    BusinessAddressArabic = x.BusinessAddressArabic,
                    BusinessAddressEnglish = x.BusinessAddressEnglish,
                    Holidays = x.Holidays,
                    Numbers = x.Numbers,
                    Opening = x.Opening,
                    Closing = x.Closing,
                    Longitude = x.Longitude,
                    Latitude = x.Latitude,
                    ProfileImageUrl = x.ProfileImageUrl,
                    ImageUrls = x.ImageUrls,
                    CategoriesModelId = x.CategoriesModelId
                }).ToListAsync();
            return new ApiResponse(200, businesses);
        }
        public async Task<ApiResponse> UpdateBusiness(int businessId, BusinessModelDto model)
        {
            var business = await _dbContext
                .BusinessModels
                .Where(x => x.UserId == model.userId)
                .FirstOrDefaultAsync(x => x.Id == businessId);
            if (business == null)
            {
                return new ApiResponse(404, "البزنس غير موجود");
            }
            if (model.ProfileImage != null)
            {
                var fileResult = await _imageService.UploadImageAsync(model.ProfileImage);
                if (fileResult.Item1 == 1)
                {
                    model.ProfileImageUrl = fileResult.Item2;
                }
                else
                {
                    return new ApiResponse(400, fileResult.Item2);
                }
            }
            if (model.Images != null)
            {
                if (model.ImageUrls == null)
                {
                    model.ImageUrls = new List<string>();
                }

                foreach (var image in model.Images)
                {
                    var fileResult = await _imageService.UploadImageAsync(image);
                    if (fileResult.Item1 == 1)
                    {
                        model.ImageUrls.Add(fileResult.Item2);
                    }
                    else
                    {
                        return new ApiResponse(400, fileResult.Item2);
                    }
                }
            }
            var mappedBusiness = _mapper.Map(model, business);
            _dbContext.BusinessModels.Update(mappedBusiness);
            await _dbContext.SaveChangesAsync();
            return new ApiResponse(200, "تم تعديل البزنس بنجاح");
        }
        public async Task<ApiResponse> GetBusinessesForBusinessOwnerAsync(string businessOwnerId)
        {
            var businesses = await _dbContext
                .BusinessModels.Where(x => x.UserId == businessOwnerId)
                .Select(x => new BusinessModel
                {
                    Id = x.Id,
                    BusinessNameArabic = x.BusinessNameArabic,
                    BusinessNameEnglish = x.BusinessNameEnglish,
                    BusinessDescriptionArabic = x.BusinessDescriptionArabic,
                    BusinessDescriptionEnglish = x.BusinessDescriptionEnglish,
                    BusinessAddressArabic = x.BusinessAddressArabic,
                    BusinessAddressEnglish = x.BusinessAddressEnglish,
                    Holidays = x.Holidays,
                    Numbers = x.Numbers,
                    Opening = x.Opening,
                    Closing = x.Closing,
                    Longitude = x.Longitude,
                    Latitude = x.Latitude,
                    ProfileImageUrl = x.ProfileImageUrl,
                    ImageUrls = x.ImageUrls,
                    CategoriesModelId = x.CategoriesModelId
                }).ToListAsync();
            return new ApiResponse(200, businesses);
        }
        public async Task<ApiResponse> GetTopFive()
        {
            var businesses = await _dbContext
                .BusinessModels
                .OrderBy(x => Guid.NewGuid())
                .Take(5)
                .Select(x => new
                {
                    Id = x.Id,
                    BusinessNameArabic = x.BusinessNameArabic,
                    BusinessNameEnglish = x.BusinessNameEnglish,
                    BusinessDescriptionArabic = x.BusinessDescriptionArabic,
                    BusinessDescriptionEnglish = x.BusinessDescriptionEnglish,
                    BusinessAddressArabic = x.BusinessAddressArabic,
                    BusinessAddressEnglish = x.BusinessAddressEnglish,
                    Holidays = x.Holidays,
                    Numbers = x.Numbers,
                    Opening = x.Opening,
                    Closing = x.Closing,
                    Longitude = x.Longitude,
                    Latitude = x.Latitude,
                    ProfileImageUrl = x.ProfileImageUrl,
                    ImageUrls = x.ImageUrls,
                    CategoriesModelId = x.CategoriesModelId
                }).ToListAsync();
            return new ApiResponse(200, businesses);
        }
    }
}
using BNS360.Core.Dto;
using BNS360.Core.Errors;
using BNS360.Core.IRepository;
using BNS360.Core.IServices;
using BNS360.Core.Models;
using BNS360.Repository.Data;
using BNS360.Repository.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Repository.Repository
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IImageService _imageService;

        public PropertyRepository(AppDbContext dbContext, IImageService imageService)
        {
            _dbContext = dbContext;
            _imageService = imageService;
        }
        public async Task<ApiResponse> AddProperty(PropertyModelDto model)
        {
            try
            {
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
                var property = model.ToEntity();
                await _dbContext.Properties.AddAsync(property);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تمت الاضافة بنجاح");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ApiResponse> DeleteProperty(int id, string UserId)
        {
            try
            {
                var property = await _dbContext
                    .Properties
                    .Where(x => x.Id == id && x.UserId == UserId)
                    .FirstOrDefaultAsync();
                if (property == null)
                    return new ApiResponse(404, "العقار غير موجود");
                if (property.ImageUrls != null)
                {
                    foreach (var imageUrl in property.ImageUrls)
                    {
                        await _imageService.DeleteImageAsync(imageUrl);
                    }
                }
                _dbContext.Properties.Remove(property);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تم الحذف بنجاح");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ApiResponse> GetProperty(int id)
        {
            try
            {
                var property = await _dbContext
                    .Properties
                    .Where(x => x.Id == id)
                    .Select(x => new
                    {
                        x.Id,
                        x.ArabicDescription,
                        x.ArabicAddress,
                        x.Numbers,
                        x.Type,
                        x.Area,
                        x.Price,
                        x.Longitude,
                        x.Latitude,
                        x.ImageUrls,
                        x.AppUser.FullName
                    }).FirstOrDefaultAsync();
                if (property == null)
                    return new ApiResponse(404, "العقار غير موجود");
                return new ApiResponse(200, property);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ApiResponse> GetProperties()
        {
            try
            {
                var properties = await _dbContext
                    .Properties
                    .Select(x => new
                    {
                        x.Id,
                        x.ArabicDescription,
                        x.ArabicAddress,
                        x.Numbers,
                        x.Type,
                        x.Area,
                        x.Price,
                        x.Longitude,
                        x.Latitude,
                        x.ImageUrls,
                        x.AppUser.FullName
                    }).ToListAsync();
                return new ApiResponse(200, properties);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ApiResponse> UpdateProperty(int id, PropertyModelDto model)
        {
            try
            {
                var property = await _dbContext
                    .Properties
                    .Where(x => x.Id == id && x.UserId == model.userId)
                    .FirstOrDefaultAsync();
                if (property == null)
                    return new ApiResponse(404, "العقار غير موجود");
                if (model.Images != null && property.ImageUrls != null && property.ImageUrls.Any())
                {
                    foreach (var oldImageUrl in property.ImageUrls)
                    {
                        await _imageService.DeleteImageAsync(oldImageUrl);
                    }
                    property.ImageUrls.Clear();
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
                model.ApplyTo(property);
                _dbContext.Properties.Update(property);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تم التعديل بنجاح");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

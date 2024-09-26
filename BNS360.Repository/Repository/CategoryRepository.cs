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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public CategoryRepository(AppDbContext context, IMapper mapper, IImageService fileService)
        {
            _dbContext = context;
            _mapper = mapper;
            _imageService = fileService;
        }
        public async Task<ApiResponse> CreateCategory(CategoryModelDto model)
        {
            var ExsistingCategory = await _dbContext.CategoryModels.FirstOrDefaultAsync(x => x.CategoryNameArabic == model.CategoryNameArabic);
            if (ExsistingCategory != null)
            {
                return new ApiResponse(400, "التصنيف موجود مسبقا");
            }
            var category = _mapper.Map<CategoryModelDto, CategoryModel>(model);

            if (model.Image != null)
            {
                var result = await _imageService.UploadImageAsync(model.Image);
                if (result.Item1 == 1)
                {
                    category.ImageUrl = result.Item2;
                }
                else
                {
                    return new ApiResponse(400, result.Item2);
                }
            }
            await _dbContext.CategoryModels.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return new ApiResponse(200, "تم اضافة التصنيف بنجاح");
        }
        public async Task<ApiResponse> DeleteCategory(int categoryId)
        {
            var category = await _dbContext.CategoryModels.FirstOrDefaultAsync(x => x.Id == categoryId);
            if (category == null)
            {
                return new ApiResponse(404, "التصنيف غير موجود");
            }
            if (!string.IsNullOrEmpty(category.ImageUrl))
            {
                await _imageService.DeleteImageAsync(category.ImageUrl);
            }
            _dbContext.CategoryModels.Remove(category);
            await _dbContext.SaveChangesAsync();
            return new ApiResponse(200, "تم حذف التصنيف بنجاح");
        }
        public async Task<ApiResponse> GetCategory(int categoryId)
        {
            var category = await _dbContext
                .CategoryModels
                .Select(x => new
                {
                    CategoryNameArabic = x.CategoryNameArabic,
                    CategoryNameEnglish = x.CategoryNameEnglish,
                    ImageUrl = x.ImageUrl
                })
                .FirstOrDefaultAsync();
            if (category == null)
            {
                return new ApiResponse(404, "التصنيف غير موجود");
            }
            return new ApiResponse(200, category);
        }
        public async Task<ApiResponse> GetCategories()
        {
            var categories = await _dbContext
                .CategoryModels
                .Select(x => new 
                {
                    Id = x.Id,
                    CategoryNameArabic = x.CategoryNameArabic,
                    CategoryNameEnglish = x.CategoryNameEnglish,
                    ImageUrl = x.ImageUrl
                })
                .ToListAsync();
            return new ApiResponse(200, categories);
        }
        public async Task<ApiResponse> UpdateCategory(int categoryId, CategoryModelDto model)
        {
            var category = await _dbContext.CategoryModels.FirstOrDefaultAsync(x => x.Id == categoryId);
            if (category == null)
            {
                return new ApiResponse(404, "التصنيف غير موجود");
            }
            if (model.Image != null)
            {
                if (!string.IsNullOrEmpty(category.ImageUrl))
                {
                    await _imageService.DeleteImageAsync(category.ImageUrl);
                }
                var result = await _imageService.UploadImageAsync(model.Image);
                if (result.Item1 == 1)
                {
                    category.ImageUrl = result.Item2;
                }
                else
                {
                    return new ApiResponse(400, result.Item2);
                }
            }
            category.CategoryNameArabic = model.CategoryNameArabic;
            category.CategoryNameEnglish = model.CategoryNameEnglish;
            await _dbContext.SaveChangesAsync();
            return new ApiResponse(200, "تم تعديل التصنيف بنجاح");
        }
    }
}

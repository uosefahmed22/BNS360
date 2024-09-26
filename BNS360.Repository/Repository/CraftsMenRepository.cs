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
    public class CraftsMenRepository : ICraftsMenRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public CraftsMenRepository(AppDbContext context, IMapper mapper, IImageService fileService)
        {
            _dbContext = context;
            _mapper = mapper;
            _imageService = fileService;
        }
        public async Task<ApiResponse> Create(CraftsMenModelDto model)
        {
            var exsistingCraftsMen = _dbContext.CraftsMen.FirstOrDefault(x => x.CraftsMenNameArabic == model.CraftsMenNameArabic);
            if (exsistingCraftsMen != null)
                return new ApiResponse(400, "البيانات موجودة مسبقا");
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
            var craftsMen = _mapper.Map<CraftsMenModel>(model);
            await _dbContext.CraftsMen.AddAsync(craftsMen);
            await _dbContext.SaveChangesAsync();
            return new ApiResponse(200, "تم الاضافة بنجاح");
        }
        public async Task<ApiResponse> Delete(int CraftsMenId,string Userid)
        {
            var craftsMen = await _dbContext
                .CraftsMen
                .Where(x => x.UserId == Userid)
                .FirstOrDefaultAsync(x => x.Id == CraftsMenId);
            if (craftsMen == null)
                return new ApiResponse(404, "البيانات غير موجودة");
            if (!string.IsNullOrEmpty(craftsMen.ProfileImageUrl))
            {
                await _imageService.DeleteImageAsync(craftsMen.ProfileImageUrl);
            }
            if (craftsMen.ImageUrls != null)
            {
                foreach (var imageUrl in craftsMen.ImageUrls)
                {
                    await _imageService.DeleteImageAsync(imageUrl);
                }
            }
            _dbContext.CraftsMen.Remove(craftsMen);
            await _dbContext.SaveChangesAsync();
            return new ApiResponse(200, "تم الحذف بنجاح");
        }
        public async Task<ApiResponse> GetAll()
        {
            var craftsMen = await _dbContext
                .CraftsMen
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
            return new ApiResponse(200, craftsMen);
        }
        public async Task<ApiResponse> GetByUserId(string UserId)
        {
            var craftsMen = await _dbContext
                .CraftsMen
                .Where(x => x.UserId == UserId)
                .Select(x => new
                {
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
                }).FirstOrDefaultAsync();
            if (craftsMen == null)
                return new ApiResponse(404, "البيانات غير موجودة");
            return new ApiResponse(200, craftsMen);
        }
        public async Task<ApiResponse> Update(int CraftsMenId, CraftsMenModelDto model)
        {
            var craftsMen = await _dbContext
                .CraftsMen
                .Where(x => x.UserId == model.UserId)
                .FirstOrDefaultAsync(x => x.Id == CraftsMenId);
            if (craftsMen == null)
                return new ApiResponse(404, "البيانات غير موجودة");
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
            _mapper.Map(model, craftsMen);
            _dbContext.CraftsMen.Update(craftsMen);
            await _dbContext.SaveChangesAsync();
            return new ApiResponse(200, "تم التعديل بنجاح");
        }
    }
}

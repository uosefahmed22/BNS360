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
    public class CraftRepository : ICraftRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public CraftRepository(AppDbContext context, IMapper mapper, IImageService imageService)
        {
            _context = context;
            _mapper = mapper;
            _imageService = imageService;
        }
        public async Task<ApiResponse> CreateCraft(CraftsModelDto model)
        {
            var existingCraft = _context
                .Crafts
                .FirstOrDefault(x => x.CraftsNameArabic == model.CraftsNameArabic);
            if (existingCraft != null)
            {
                return new ApiResponse(400, "موجود بالفعل");
            }
            var craft = _mapper.Map<CraftsModel>(model);
            if (model.Image != null)
            {
                var result = await _imageService.UploadImageAsync(model.Image);
                if (result.Item1 == 1)
                {
                    craft.ImageUrl = result.Item2;
                }
                else
                {
                    return new ApiResponse(400, result.Item2);
                }
            }
            await _context.Crafts.AddAsync(craft);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "تم اضافة الحرف بنجاح");
        }
        public async Task<ApiResponse> DeleteCraft(int craftId)
        {
            var craft = await _context.Crafts.FirstOrDefaultAsync(x => x.Id == craftId);
            if (craft == null)
            {
                return new ApiResponse(404, "الحرف غير موجود");
            }
            if (!string.IsNullOrEmpty(craft.ImageUrl))
            {
                await _imageService.DeleteImageAsync(craft.ImageUrl);
            }
            _context.Crafts.Remove(craft);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "تم حذف الحرف بنجاح");
        }
        public async Task<ApiResponse> GetCraft(int craftId)
        {
            var craft = await _context
                .Crafts
                .Select(x => new
                {
                    x.CraftsNameArabic,
                    x.CraftsNameEnglish,
                    x.ImageUrl
                })
                .FirstOrDefaultAsync();
            if (craft == null)
            {
                return new ApiResponse(404, "الحرف غير موجود");
            }
            return new ApiResponse(200, craft);
        }
        public async Task<ApiResponse> GetCrafts()
        {
            var crafts = await _context
                .Crafts
                .Select(x => new
                {
                    x.Id,
                    x.CraftsNameArabic,
                    x.CraftsNameEnglish,
                    x.ImageUrl
                })
                .ToListAsync();
            if (crafts == null)
            {
                return new ApiResponse(404, "لا يوجد حرف");
            }
            return new ApiResponse(200, crafts);
        }
        public async Task<ApiResponse> UpdateCraft(int craftId, CraftsModelDto model)
        {
            var craft = await _context.Crafts.FirstOrDefaultAsync(x => x.Id == craftId);
            if (craft == null)
            {
                return new ApiResponse(404, "الحرف غير موجود");
            }

            if (model.Image != null)
            {
                if (!string.IsNullOrEmpty(craft.ImageUrl))
                {
                    await _imageService.DeleteImageAsync(craft.ImageUrl);
                }
                var result = await _imageService.UploadImageAsync(model.Image);
                if (result.Item1 == 1)
                {
                    craft.ImageUrl = result.Item2;
                }
                else
                {
                    return new ApiResponse(400, result.Item2);
                }
            }
            craft.CraftsNameArabic = model.CraftsNameArabic;
            craft.CraftsNameEnglish = model.CraftsNameEnglish;
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "تم تعديل الحرف بنجاح");
        }
    }
}
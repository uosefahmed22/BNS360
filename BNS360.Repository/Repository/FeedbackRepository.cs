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
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public FeedbackRepository(AppDbContext dbContext,IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<ApiResponse> AddFeedbackForBusiness(string userId, int businessId, string feedback, int rating)
        {
            try
            {
                var feedbackModel = new FeedbackModel
                {
                    UserId = userId,
                    BusinessModelId = businessId,
                    Feedback = feedback,
                    rating = rating
                };
                await _dbContext.Feedbacks.AddAsync(feedbackModel);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تم اضافة التقييم بنجاح");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> AddFeedbackForCraftsMen(string userId, int craftsMenId, string feedback, int rating)
        {
            try
            {
                var feedbackModel = new FeedbackModel
                {
                    UserId = userId,
                    CraftsMenModelId = craftsMenId,
                    Feedback = feedback,
                    rating = rating
                };
                await _dbContext.Feedbacks.AddAsync(feedbackModel);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تم اضافة التقييم بنجاح");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> DeleteBusinessFeedback(string userId, int feedbackId)
        {
            try
            {
                var feedback =await _dbContext.Feedbacks.FirstOrDefaultAsync(x => x.Id == feedbackId && x.UserId == userId);
                if (feedback == null)
                {
                    return new ApiResponse(404, "لا يوجد تقييم بهذا الرقم");
                }
                _dbContext.Feedbacks.Remove(feedback);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تم حذف التقييم بنجاح");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> GetFeedbackForBusiness(int businessId)
        {
            try
            {
                var feedbacks =await _dbContext
                    .Feedbacks
                    .Where(x => x.BusinessModelId == businessId)
                    .Select(x => new
                    {
                        x.Id,
                        x.Feedback,
                        x.rating,
                        x.CreatedDate,
                        x.AppUser.FullName
                    })
                    .ToListAsync();
                return new ApiResponse(200, feedbacks);
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> DeleteCraftsMenFeedback(string userId, int feedbackId)
        {
            try
            {
                var feedback =await _dbContext.Feedbacks.FirstOrDefaultAsync(x => x.Id == feedbackId && x.UserId == userId);
                if (feedback == null)
                {
                    return new ApiResponse(404, "لا يوجد تقييم بهذا الرقم");
                }
                _dbContext.Feedbacks.Remove(feedback);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تم حذف التقييم بنجاح");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> GetFeedbackForCraftsMen(int craftsMenId)
        {
            try
            {
                var feedbacks =await _dbContext
                    .Feedbacks
                    .Where(x => x.CraftsMenModelId == craftsMenId)
                    .Select(x => new
                    {
                        x.Id,
                        x.Feedback,
                        x.rating,
                        x.CreatedDate,
                        x.AppUser.FullName
                    })
                    .ToListAsync();
                return new ApiResponse(200, feedbacks);
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
    }
}

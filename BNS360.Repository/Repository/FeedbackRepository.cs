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
                        x.AppUser.FullName,
                        x.AppUser.ImageUrl
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
                        x.AppUser.FullName,
                        x.AppUser.ImageUrl
                    })
                    .ToListAsync();
                return new ApiResponse(200, feedbacks);
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> GetBusinessReviewsAndRatingsSummaryAsync(int businessId)
        {
            try
            {
                var reviews = await _dbContext.Feedbacks
                    .Where(r => r.BusinessModelId == businessId)
                    .ToListAsync();

                if (reviews == null || reviews.Count == 0)
                {
                    return new ApiResponse(200, new ReviewAndRatingSummaryResponse
                    {
                        TotalReviews = 0,
                        AverageRating = 0,
                        FiveStars = 0,
                        FourStars = 0,
                        ThreeStars = 0,
                        TwoStars = 0,
                        OneStars = 0
                    });
                }

                double totalRating = reviews.Sum(r => r.rating);
                double averageRating = totalRating / reviews.Count;

                int fiveStars = reviews.Count(r => r.rating >= 4.5 && r.rating <= 5);
                int fourStars = reviews.Count(r => r.rating >= 3.5 && r.rating < 4.5);
                int threeStars = reviews.Count(r => r.rating >= 2.5 && r.rating < 3.5);
                int twoStars = reviews.Count(r => r.rating >= 1.5 && r.rating < 2.5);
                int oneStars = reviews.Count(r => r.rating >= 0.5 && r.rating < 1.5);

                return new ApiResponse(200, new ReviewAndRatingSummaryResponse
                {
                    TotalReviews = reviews.Count,
                    AverageRating = averageRating,
                    FiveStars = fiveStars,
                    FourStars = fourStars,
                    ThreeStars = threeStars,
                    TwoStars = twoStars,
                    OneStars = oneStars
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> GetCraftsmenReviewsAndRatingsSummaryAsync(int craftsMenId)
        {
            try
            {
                var reviews = await _dbContext.Feedbacks
                    .Where(r => r.CraftsMenModelId == craftsMenId)
                    .ToListAsync();

                if (reviews == null || reviews.Count == 0)
                {
                    return new ApiResponse(200, new ReviewAndRatingSummaryResponse
                    {
                        TotalReviews = 0,
                        AverageRating = 0,
                        FiveStars = 0,
                        FourStars = 0,
                        ThreeStars = 0,
                        TwoStars = 0,
                        OneStars = 0
                    });
                }

                double totalRating = reviews.Sum(r => r.rating);
                double averageRating = totalRating / reviews.Count;

                int fiveStars = reviews.Count(r => r.rating >= 4.5 && r.rating <= 5);
                int fourStars = reviews.Count(r => r.rating >= 3.5 && r.rating < 4.5);
                int threeStars = reviews.Count(r => r.rating >= 2.5 && r.rating < 3.5);
                int twoStars = reviews.Count(r => r.rating >= 1.5 && r.rating < 2.5);
                int oneStars = reviews.Count(r => r.rating >= 0.5 && r.rating < 1.5);

                return new ApiResponse(200, new ReviewAndRatingSummaryResponse
                {
                    TotalReviews = reviews.Count,
                    AverageRating = averageRating,
                    FiveStars = fiveStars,
                    FourStars = fourStars,
                    ThreeStars = threeStars,
                    TwoStars = twoStars,
                    OneStars = oneStars
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
    }
}

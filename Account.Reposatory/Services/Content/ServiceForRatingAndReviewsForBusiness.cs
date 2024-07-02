using Account.Apis.Errors;
using Account.Core.Dtos.RatingAndReviewDto;
using Account.Core.Dtos.RatingAndReviewDto.Account.Core.Dtos.RatingAndReviewDto;
using Account.Core.IServices.Content;
using Account.Core.Models.Account;
using Account.Core.Models.Content.RatingReview;
using Account.Reposatory.Data.Context;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Reposatory.Services.Content
{
    public class ServiceForRatingAndReviewsForBusiness : IServiceForRatingAndReviewsForBusiness
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public ServiceForRatingAndReviewsForBusiness()
        {

        }
        public ServiceForRatingAndReviewsForBusiness(AppDBContext context, IMapper mapper,UserManager<AppUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<ApiResponse> AddAsync(RatingAndReviewModelForBusinessDto savedModel)
        {
            try
            {
                var entity = _mapper.Map<RatingAndReviewModelForBusiness>(savedModel);

                _context.ratingAndReviewModelForBusinesses.Add(entity);
                await _context.SaveChangesAsync();
                return new ApiResponse(200, "Record added successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, $"Failed to add record: {ex.Message}");
            }
        }
        public async Task<IEnumerable<ReviewAndRatingResponse>> GetReviewsAndRatings(int businessId)
        {
            try
            {
                var reviews = await _context.ratingAndReviewModelForBusinesses
                                            .Where(r => r.businessId == businessId)
                                            .ToListAsync();

                var responseList = new List<ReviewAndRatingResponse>();

                foreach (var review in reviews)
                {
                    var user = await _userManager.FindByIdAsync(review.userId);

                    var response = new ReviewAndRatingResponse
                    {
                        Id = review.Id,
                        Review = review.Review,
                        Rating = review.Rating,
                        dateTime = review.CreatedAt,
                        PhotoUrl = user?.profileImageName,
                        UserName = user?.DisplayName,
                        UserId = user?.Id ?? "-1"
                    };

                    if (user == null)
                    {
                        response.UserName = "Deleted account";
                        response.PhotoUrl = null;
                    }

                    responseList.Add(response);
                }

                return responseList;
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                throw;
            }
        }
        public async Task<ApiResponse> RemoveAsync(string userId, int businessId, int reviewAndRatingId)
        {
            try
            {
                var reviewAndRating = await _context.ratingAndReviewModelForBusinesses
                    .FirstOrDefaultAsync(r => r.Id == reviewAndRatingId && r.userId == userId && r.businessId == businessId);

                if (reviewAndRating == null)
                    return new ApiResponse(404, "Record not found.");

                _context.ratingAndReviewModelForBusinesses.Remove(reviewAndRating);
                await _context.SaveChangesAsync();

                return new ApiResponse(200, "Record removed successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, $"Failed to remove record: {ex.Message}");
            }
        }
        public async Task<ReviewAndRatingSummaryResponse> GetReviewsAndRatingsForBusinessWithDetailsAsync(int businessId)
        {
            try
            {
                var reviews = await _context.ratingAndReviewModelForBusinesses
                                             .Where(r => r.businessId == businessId)
                                             .ToListAsync();

                if (reviews == null || reviews.Count == 0)
                {
                    return new ReviewAndRatingSummaryResponse
                    {
                        TotalReviews = 0,
                        AverageRating = 0,
                        FiveStars = 0,
                        FourStars = 0,
                        ThreeStars = 0,
                        TwoStars = 0,
                        OneStars = 0
                    };
                }

                double totalRating = reviews.Sum(r => r.Rating ?? 0);
                double averageRating = totalRating / reviews.Count;
                averageRating = Math.Min(averageRating, 5);

                int fiveStars = reviews.Count(r => (r.Rating ?? 0) >= 4.5 && (r.Rating ?? 0) <= 5);
                int fourStars = reviews.Count(r => (r.Rating ?? 0) >= 3.5 && (r.Rating ?? 0) < 4.5);
                int threeStars = reviews.Count(r => (r.Rating ?? 0) >= 2.5 && (r.Rating ?? 0) < 3.5);
                int twoStars = reviews.Count(r => (r.Rating ?? 0) >= 1.5 && (r.Rating ?? 0) < 2.5);
                int oneStars = reviews.Count(r => (r.Rating ?? 0) >= 0.5 && (r.Rating ?? 0) < 1.5);

                return new ReviewAndRatingSummaryResponse
                {
                    TotalReviews = reviews.Count,
                    AverageRating = averageRating,
                    FiveStars = fiveStars,
                    FourStars = fourStars,
                    ThreeStars = threeStars,
                    TwoStars = twoStars,
                    OneStars = oneStars
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ApiResponse> RemoveReviewForAdminAsync(int reviewAndRatingId)
        {
            try
            {
                var reviewAndRating = await _context.ratingAndReviewModelForBusinesses
                    .FirstOrDefaultAsync(r => r.Id == reviewAndRatingId);

                if (reviewAndRating == null)
                    return new ApiResponse(404, "Record not found.");

                _context.ratingAndReviewModelForBusinesses.Remove(reviewAndRating);
                await _context.SaveChangesAsync();

                return new ApiResponse(200, "Record removed successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, $"Failed to remove record: {ex.Message}");
            }
        }

    }
}

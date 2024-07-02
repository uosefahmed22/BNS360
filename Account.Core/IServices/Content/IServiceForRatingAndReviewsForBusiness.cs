using Account.Apis.Errors;
using Account.Core.Dtos.RatingAndReviewDto;
using Account.Core.Dtos.RatingAndReviewDto.Account.Core.Dtos.RatingAndReviewDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Core.IServices.Content
{
    public interface IServiceForRatingAndReviewsForBusiness
    {
        Task<IEnumerable<ReviewAndRatingResponse>> GetReviewsAndRatings(int businessId);
        Task<ApiResponse> AddAsync(RatingAndReviewModelForBusinessDto savedModel);
        Task<ApiResponse> RemoveAsync(string userId, int businessId, int reviewAndRatingId);
        Task<ReviewAndRatingSummaryResponse> GetReviewsAndRatingsForBusinessWithDetailsAsync(int businessId);
        Task<ApiResponse> RemoveReviewForAdminAsync(int reviewAndRatingId);
    }
}

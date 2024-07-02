using Account.Apis.Errors;
using Account.Core.Dtos.BusinessDto;
using Account.Core.Dtos.FavoirteDto;
using Account.Core.Dtos.RatingAndReviewDto;
using Account.Core.Dtos.RatingAndReviewDto.Account.Core.Dtos.RatingAndReviewDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Core.IServices.Content
{
    public interface IServiceForRatingAndReviewsForCraftsmen
    {
        Task<IEnumerable<ReviewAndRatingResponse>> GetReviewsAndRatings(int craftsmanId);
        Task<ApiResponse> AddAsync(RatingAndReviewModelForCraftsmenDto savedModel);
        Task<ApiResponse> RemoveAsync(string userId, int craftsmanId, int reviewAndRatingId);
        Task<Dtos.RatingAndReviewDto.Account.Core.Dtos.RatingAndReviewDto.ReviewAndRatingSummaryResponse> GetReviewsAndRatingsForCraftsmanAsync(int craftsmanId);
        Task<ApiResponse> RemoveReviewForAdminAsync(int reviewAndRatingId);
    }
}

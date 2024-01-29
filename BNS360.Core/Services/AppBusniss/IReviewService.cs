
using BNS360.Core.Dtos.Response.AppBusniss;
using BNS360.Core.Entities;
using BNS360.Core.Helpers;

namespace BNS360.Core.Services.AppBusniss
{
    public interface IReviewService
    {
        Task<ReviewSummary> GetReviewSummaryAsync<T>(Guid id) where T : IHaveReviews;
    }
}
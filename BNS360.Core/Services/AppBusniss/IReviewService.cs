
using BNS360.Core.Dtos.Response.AppBusniss;
using BNS360.Core.Entities;
using BNS360.Core.Helpers;
using System.Linq.Expressions;

namespace BNS360.Core.Services.AppBusniss
{
    public interface IReviewService
    {
        Task<ReviewSummary> GetReviewSummaryAsync<T>(Guid id) where T : MainEntity;
        ReviewSummary GetReviewSummary(IList<Review>? reviews);
        Task<List<Review>> GetReviewsAsync(Guid id, int pageNumber, int size);  
    }
}
using BNS360.Core.Dtos.Response.AppBusniss;
using BNS360.Core.Entities;
using BNS360.Core.Helpers;
using BNS360.Core.Services.AppBusniss;
using BNS360.Reposatory.Data.AppBusniss;
using Microsoft.EntityFrameworkCore;

namespace BNS360.Reposatory.Repositories.Shared
{
    public class ReviewService : IReviewService
    {
        private readonly AppBusnissDbContext _context;

        public ReviewService(AppBusnissDbContext context)
        {
            _context = context;
        }

        public async Task<ReviewSummary> GetReviewSummaryAsync<T>(Guid id) where T : IHaveReviews
        {
            if (typeof(T) == typeof(Busniss))
            {
                var reviewSummary = await _context.Reviews.AsNoTracking()
                    .Where(r => r.BusnissId == id)
                    .GroupBy(r => r.BusnissId)
                    .Select(g => new ReviewSummary(
                        g.Count(),
                        (float)Math.Min(5, Math.Round(g.Average(r => r.Rate), 1))))
                    .FirstOrDefaultAsync();


                return reviewSummary ?? throw new Exception("no reviews for this busniss");
            }
            return new ReviewSummary(0,0);
        }
    }
}

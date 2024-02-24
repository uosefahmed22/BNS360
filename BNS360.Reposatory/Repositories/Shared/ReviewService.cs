using BNS360.Core.CustemExceptions;
using BNS360.Core.Dtos.Response.AppBusniss;
using BNS360.Core.Entities;
using BNS360.Core.Helpers;
using BNS360.Core.Services.AppBusniss;
using BNS360.Reposatory.Data.AppBusniss;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BNS360.Reposatory.Repositories.Shared
{
    public class ReviewService : IReviewService
    {
        private readonly AppBusnissDbContext _context;
        private readonly object _locker = new object(); 
        public ReviewService(AppBusnissDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetReviewsAsync(Guid busnissId, int pageNumber, int size)
        {
             var result = await _context.Reviews
                .Where(r => r.BusnissId == busnissId)
                .Skip((pageNumber - 1) * size)
                .Take(size)
                .ToListAsync();

            if (result is null)
            {
                throw new ItemNotFoundException("No Reviews Was Found");
            }

            return result;
        }

        public ReviewSummary GetReviewSummary(IList<Review>? reviews)
        {
            if (reviews is null || !reviews.Any()) 
                return new ReviewSummary(0, 0);

            return new ReviewSummary(
                reviews.Count,
                Math.Min(5, (float)Math.Round(reviews.Average(r => r.Rate), 1)));
        }

        public IQueryable<ReviewSummary> GetReviewSummaryAsQueryAsync<T>(Guid id) where T : MainEntity
        {
            if (typeof(T) == typeof(Busniss))
            {
                lock(_locker)
                {
                    

                    var resultQuery = _context.Reviews.AsNoTracking()
                        .Where(r => r.BusnissId == id)
                        .GroupBy(r => r.BusnissId)
                        .Select(g => new ReviewSummary(
                            g.Count(),
                            (float)Math.Min(5, Math.Round(g.Average(r => r.Rate), 1))
                            )).AsQueryable();
                    return resultQuery;

                }

            }
            return  _context.Reviews.AsNoTracking()
                .Where(r => r.BusnissId == id)
                .Select(r => new ReviewSummary(0,0)).AsQueryable();
        }

        public async Task<ReviewSummary> GetReviewSummaryAsync<T>(Guid id) where T : MainEntity
        {
            if (typeof(T) == typeof(Busniss))
            {
                var reviewSummary = await GetReviewSummaryAsQueryAsync<Busniss>(id)
                    .FirstOrDefaultAsync();
                return reviewSummary ?? new ReviewSummary(0, 0);
            }
            return new ReviewSummary(0,0);
        }
    }
}

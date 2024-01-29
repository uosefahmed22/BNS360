using BNS360.Core.Abstractions;
using BNS360.Core.CustemExceptions;
using BNS360.Core.Dtos.Request;
using BNS360.Core.Dtos.Response.AppBusniss;
using BNS360.Core.Entities;
using BNS360.Core.Errors;
using BNS360.Core.Services.AppBusniss;
using BNS360.Reposatory.Data.AppBusniss;
using BNS360.Reposatory.Repositories.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Reposatory.Repositories.Repositories
{
    public class BusnissRepository : IBusnissRepository
    {
        private readonly AppBusnissDbContext _context;
        private readonly IReviewService _reviewService;
        public BusnissRepository(AppBusnissDbContext context, IReviewService reviewService)
        {
            _context = context;
            _reviewService = reviewService;
        }

        public async Task<ApiResponse> CreateAsync(BusnisRequest request,Guid userId)
        {
            var busnissExsist = await _context.Busnisses
                .Where(b => b.UserId == userId)
                .AnyAsync();
            var validCategoryId = await _context.Categories.Where(c => c.Id == request.CategoryId).AnyAsync();
            if (busnissExsist)
            {
                return new ApiResponse
                {
                    StatusCode = 500,
                    Message = $"user with ID: {userId} already have a Busniss"
                };
            }
            if (!validCategoryId)
            {
                return new ApiResponse
                {
                    StatusCode = 500,
                    Message = $"CategoryID: {request.CategoryId} is Invalid"
                };
            }
            Busniss busniss = new()
            { 
                UserId = userId,
                Name = request.Name,
                About = request.About,
                CategoryId = request.CategoryId,
                Location = new() { Address = request.Location.Address,
                    Latitude = request.Location.Latitude,
                    Longitude = request.Location.Longitude,
                },
                ContactInfo = new() {
                    FirstPhoneNumber = request.ContactInfo.PhoneNumbers[0],
                    SecoundPhoneNumber = request.ContactInfo.PhoneNumbers[0],
                    ThirdPhoneNumber = request.ContactInfo.PhoneNumbers[0],
                    EmailAddress = request.ContactInfo.EmailAddress,
                    SiteUrl = request.ContactInfo.SiteUrl,
                },
            };
            foreach (var item in request.WorkTime)
            {
                busniss.WorkTime.Add(new()
                {
                    Day = item.Day,
                    Start = item.Strart,
                    End = item.End
                });
            }
            
            _context.Busnisses.Add(busniss);

            await _context.SaveChangesAsync();

            return new ApiResponse(200);
        }

        public async Task<IReadOnlyList<BusnissReponse>> GetAllBusnissWithCategoryIdAsync(Guid categoryId)
        {
            var result = await _context.Busnisses.AsNoTracking()
                .Where(b => b.CategoryId == categoryId)
                .Select(b => new BusnissReponse
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.About, 
                    ReviewSummary = new ReviewSummary(b.Reviews != null ? b.Reviews.Count() : 0,
                    b.Reviews != null && b.Reviews.Any() ? Math.Min(5, (float)Math.Round(b.Reviews.Average(r => r.Rate),1)) : 0),
                    ProfilePictureUrl = b.ProfilePictureUrl
                })
                .ToListAsync();
            
            return result;
        }

        public async Task<ApiResponse> GetByIdAsync(Guid Id)
        {
            var busniss = await _context.Busnisses.AsNoTracking()
                .Where(b => b.Id == Id)
                .Include(b => b.Location)
                .Include(b => b.WorkTime)
                .Include(b => b.ContactInfo)
                .FirstOrDefaultAsync();
            
            ItemNotFoundException.ThrowIfNull(busniss,nameof(busniss));

            

            return new BusnissDetails
            {
                Name = busniss.Name,
                Description = busniss.About,
                LocationInfo = busniss.Location!,
                WorkTime = busniss.WorkTime.ToList(),
                Reviews = busniss.Reviews?.ToList(),
                ContactInfo = busniss.ContactInfo!,
                ReviewSummary = await _reviewService.GetReviewSummaryAsync<Busniss>(Id)
            };
        }

        public async Task<IReadOnlyList<BusnissReponse>> GetRecommendedAsync()
        {
            var result = await _context.Categories.AsNoTracking()
            .Where(c => c.Busnisses != null && c.Busnisses.Any())  
            .Include(c => c.Busnisses)
            .SelectMany(c => c.Busnisses!
            .Where(b => b.Reviews != null && b.Reviews.Any())
            .OrderByDescending(b => b.Reviews!.Count())
            .Take(1)
            .Select(business => new BusnissReponse
            {
                Id = business.Id,
                Name = business.Name,
                Description = business.About,
                ReviewSummary = new ReviewSummary(
                    business.Reviews != null ? business.Reviews.Count() : 0,
                    business.Reviews != null && business.Reviews.Any() ?
                    Math.Min(5, (float)Math.Round(business.Reviews.Average(r => r.Rate),1)) : 0),

                ProfilePictureUrl = business.ProfilePictureUrl
            }))
            .ToListAsync();

            return result;
        }
    }
}
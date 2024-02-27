using BNS360.Core.Abstractions;
using BNS360.Core.CustemExceptions;
using BNS360.Core.Dtos.Request;
using BNS360.Core.Dtos.Response;
using BNS360.Core.Dtos.Response.AppBusniss;
using BNS360.Core.Entities;
using BNS360.Core.Errors;
using BNS360.Core.Helpers;
using BNS360.Core.Services;
using BNS360.Core.Services.AppBusniss;
using BNS360.Core.Services.Authentication;
using BNS360.Core.Services.Shared;
using BNS360.Core.Specifications;
using BNS360.Reposatory.Data.AppBusniss;
using BNS360.Reposatory.Repositories.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace BNS360.Reposatory.Repositories.Repositories;

public class BusnissRepository
    (
    AppBusnissDbContext context,
    IReviewService reviewService,
    IWorkTimeService workTimeService,
    IFavoriteService favouriteService,
    IFileService fileService,
    IDistanceService distanceService,
    IDateTimeProvider dateTimeProvider,
    IUserService userService)
    : IBusnissRepository
{

    #region Dependencies
    private readonly AppBusnissDbContext _context = context;
    private readonly IReviewService _reviewService = reviewService;
    private readonly IWorkTimeService _workTimeService = workTimeService;
    private readonly IFavoriteService _favouriteService = favouriteService;
    private readonly IFileService _fileService = fileService;
    private readonly IDistanceService _distanceService = distanceService;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IUserService _userService = userService;

    #endregion

    #region Queries
    public async Task<ApiResponse> GetByIdAsync(Guid Id)
    {
        var busniss = await _context.Busnisses.AsNoTracking()
            .Where(b => b.Id == Id)
            .Include(b => b.Location)
            .Include(b => b.WorkTime)
            .Include(b => b.ContactInfo)
            .Include(b => b.Category)
            .AsSplitQuery()
            .FirstOrDefaultAsync();

        if (busniss is null)
        {
            return new ApiResponse(StatusCodes.Status404NotFound, $"Busniss With ID {Id} Was Not Found");
        }

        var reviewsSummary = await _reviewService.GetReviewSummaryAsync<Business>(Id);

        return new BusnissDetails
        {
            Id = busniss.Id,
            NameAR = busniss.NameAR,
            NameENG = busniss.NameENG,
            DescriptionENG = busniss.AboutENG,
            Description = busniss.AboutAR,
            Category = busniss.Category!.Name,
            LocationInfo = busniss.Location!,
            WorkTime = busniss.WorkTime.ToList(),
            ContactInfo = busniss.ContactInfo!,
            ReviewSummary = reviewsSummary,
            ProfilePicture = _fileService.GetAbsoluteFilePath(busniss.ProfilePictureUrl),
            AlbumPictures = _fileService.GetAlbumPictures(busniss.AlbumUrl)
        };
    }
    public async Task<IReadOnlyList<BusnissReponse>> GetAllBusnissWithCategoryIdAsync(Guid categoryId)
    {
        if (!await _context.Categories.AnyAsync(c => c.Id == categoryId))
            throw new ItemNotFoundException("Category Was Not Found");

        var result = await _context.Busnisses.AsNoTracking()
            .Where(b => b.CategoryId == categoryId)
            .Select(busniss => busniss.MapToBusnissResponse(_reviewService, _fileService))
            .ToListAsync();

        return result;
    }
    public async Task<IReadOnlyList<BusnissReponse>> GetRecommendedAsync()
    {
        var result = await _context.Categories.AsNoTracking()
            .Where(c => c.Busnisses != null && c.Busnisses.Any())
            .Include(c => c.Busnisses)
            .SelectMany(c => c.Busnisses!
                .Where(b => b.Reviews != null && b.Reviews.Any())
                .OrderByDescending(b => b.Reviews!.Count())
                .Take(10)
                .OrderByDescending(b => b.Reviews!.Average(r => r.Rate))
                .Take(1)
                .ToList())
            .Select(busniss => busniss.MapToBusnissResponse(_reviewService, _fileService))
            .ToListAsync();
        return result;
    }

    public async Task<ApiResponse> Paganate(int pageNumber, int pageSize, Guid categoryId)
    {
        var spc = new GetPageSpecification<Business>(
            pageNumber,
            pageSize,
            b => b.CategoryId == categoryId);
        var items = await SpecificationEvaluator<Business>.
            Evaluate(_context.Busnisses.AsNoTracking().AsQueryable(), spc).Include(b => b.Reviews)
            .Select(busniss => busniss.MapToBusnissResponse(_reviewService, _fileService))
            .ToListAsync();

        var result = new PagenationResponse<BusnissReponse>()
        {
            TotalCount = await _context.Busnisses.Where(b => b.CategoryId == categoryId).CountAsync(),
            CurrentPage = pageNumber,
            Items = items,
        };
        return result;
    }
    public async Task<ApiResponse> GetReviewsAsync(Guid id, int pageNumber, int size)
    {

        var reviews = await _reviewService.GetReviewsAsync(id, pageNumber, size);

        List<ReviewResponse> items = new();
        foreach (var review in reviews)
        {
            var userInfo = await _userService.GetMainProfileIfoAsync(review.UserId);
            items.Add(new ReviewResponse
            {
                UserName = userInfo.username,
                UserProfilePicture = _fileService.GetAbsoluteFilePath(userInfo.profilePictureUrl),
                Id = review.Id,
                Comment = review.Comment,
                Rate = review.Rate,
                LastModefied = review.LastModefied ?? default(DateTime),
            });
        }
        return new PagenationResponse<ReviewResponse>()
        {
            Items = items,
            CurrentPage = pageNumber,
            TotalCount = await _context.Reviews.Where(r => r.BusnissId == id).CountAsync(),
        };
    }
    public async Task<List<BusnissReponse>?> SearchAsync(string key, Guid categoryId)
    {
        if (!await _context.Categories.AnyAsync(c => c.Id == categoryId))
        {
            return new List<BusnissReponse>();
        }

        var result = await _context.Busnisses
            .AsNoTracking()
            .Where(b => b.CategoryId == categoryId && b.NameAR.ToLower().Contains(key.ToLower()))
            .Select(busniss => busniss.MapToBusnissResponse(_reviewService, _fileService))
            .ToListAsync();

        return result;
    }
    #region Filters
    public async Task<ApiResponse> FilterByDistance(Guid categoryId, CurrentLocation location, int pageNumber, int size)
    {

        if (!await _context.Categories.AnyAsync(c => c.Id == categoryId))
        {
            return new ApiResponse(StatusCodes.Status404NotFound, "Category Was Not Found");
        }

        var busniss = await _context.Busnisses
            .Where(b => b.CategoryId == categoryId)
            .Include(b => b.Location)
            .AsSplitQuery()
            .ToListAsync();

        var items = busniss
            .OrderBy(b => _distanceService.FindDistance(location, b.Location!))
            .Skip((pageNumber - 1) * size)
            .Take(size)
            .Select(async b => new BusnissReponse
            {
                Id = b.Id,
                NameAR = b.NameAR,
                DescriptionAR = b.AboutAR,
                ProfilePictureUrl = _fileService.GetAbsoluteFilePath(b.ProfilePictureUrl),
                ReviewSummary = await _reviewService.GetReviewSummaryAsync<Business>(b.Id),
                Distance = Math.Round(_distanceService.FindDistance(location, b.Location!), 1)
            })
            .ToList();

        var x = await Task.WhenAll(items);

        var result = new PagenationResponse<BusnissReponse>()
        {
            Items = x.ToList(),
            TotalCount = busniss.Count,
            CurrentPage = pageNumber,
        };
        return result;
    }

    public async Task<ApiResponse> FilterByActive(Guid categoryId, int pageNUmber, int size)
    {
        if (!await _context.Categories.AnyAsync(c => c.Id == categoryId))
        {
            return new ApiResponse(StatusCodes.Status404NotFound, "Category Was Not Found");
        }

        Expression<Func<Business, bool>> predicate = b => b.CategoryId == categoryId
        && b.WorkTime.Any(wt => wt.Day == _dateTimeProvider.Now.DayOfWeek &&
            wt.Start <= _dateTimeProvider.CurrentTime &&
            wt.End > _dateTimeProvider.CurrentTime);
        var items = await _context.Busnisses
           .AsNoTracking()
            .Where(predicate)
           .Skip((pageNUmber - 1) * size)
           .Take(size)
           .Select(b => new BusnissReponse
           {
               Id = b.Id,
               NameAR = b.NameAR,
               DescriptionAR = b.AboutAR,
               ProfilePictureUrl = _fileService.GetAbsoluteFilePath(b.ProfilePictureUrl),
               ReviewSummary = _reviewService.GetReviewSummary(b.Reviews)
           })
           .ToListAsync();


        return new PagenationResponse<BusnissReponse>()
        {
            CurrentPage = pageNUmber,
            TotalCount = await _context.Busnisses.CountAsync(predicate),
            Items = items
        };
    }

    public async Task<ApiResponse> FilterByRatings(Guid categoryId, int pageNUmber, int size)
    {
        if (!await _context.Categories.AnyAsync(c => c.Id == categoryId))
        {
            return new ApiResponse(StatusCodes.Status404NotFound, "Category Was Not Found");
        }

        var items = await _context.Busnisses.AsNoTracking()
            .Where(b => b.CategoryId == categoryId)
            .Where(b => b.Reviews != null && b.Reviews.Any())
            .OrderByDescending(b => b.Reviews!.Count)
            .Skip(pageNUmber - 1)
            .Take(size)
            .OrderByDescending(b => b.Reviews!.Average(r => r.Rate))
            .Select(b => new BusnissReponse
            {
                Id = b.Id,
                NameAR = b.NameAR,
                DescriptionAR = b.AboutAR,
                ProfilePictureUrl = _fileService.GetAbsoluteFilePath(b.ProfilePictureUrl),
                ReviewSummary = _reviewService.GetReviewSummary(b.Reviews),
            })
            .AsSplitQuery()
            .ToListAsync();

        return new PagenationResponse<BusnissReponse>()
        {
            CurrentPage = pageNUmber,
            TotalCount = await _context.Busnisses.Where(b => b.CategoryId == categoryId).CountAsync(),
            Items = items
        };
    }

    #endregion


    #endregion

    #region Commands

    public async Task<ApiResponse> CreateAsync(BusnisRequest request, Guid userId)
    {
        var busnissExsist = await _context.Busnisses
            .Where(b => b.UserId == userId)
            .AnyAsync();

        var validCategoryId = await _context.Categories.Where(c => c.Id == request.CategoryId).AnyAsync();
        if (busnissExsist)
        {
            return new ApiResponse
            {
                StatusCode = 409,
                Message = $"user with ID: {userId} already have a Busniss"
            };
        }
        if (!validCategoryId)
        {
            return new ApiResponse
            {
                StatusCode = 400,
                Message = $"CategoryID: {request.CategoryId} is InValid"
            };
        }
        Business busniss = new()
        {
            UserId = userId,
            NameAR = request.Name,
            AboutAR = request.About,
            CategoryId = request.CategoryId,
            Location = new()
            {
                Address = request.Location.Address,
                Latitude = request.Location.Latitude,
                Longitude = request.Location.Longitude,
            },
            ContactInfo = new()
            {
                FirstPhoneNumber = request.ContactInfo.FirstPhoneNumber,
                SecoundPhoneNumber = request.ContactInfo.SecoundPhoneNumber,
                ThirdPhoneNumber = request.ContactInfo.ThirdPhoneNumber,
                EmailAddress = request.ContactInfo.EmailAddress,
                SiteUrl = request.ContactInfo.SiteUrl,
            },
        };

        busniss.WorkTime = WorkTime.SetBusnissWorkTime(request.WorkTime, busniss.Id).ToList();

        _context.Busnisses.Add(busniss);

        await _context.SaveChangesAsync();

        return new ApiResponse(200, busniss.Id.ToString());
    }
    public async Task<ApiResponse> AddReviewAsync(ReviewRequest request, Guid id)
    {
        var busniss = await _context.Busnisses.FirstOrDefaultAsync(x => x.Id == request.BusnissId);

        ItemNotFoundException.ThrowIfNull(busniss, nameof(busniss));
        _context.Reviews.Add(new Review
        {
            Comment = request.Comment,
            Rate = request.Rate,
            UserId = id,
            BusnissId = request.BusnissId
        });
        await _context.SaveChangesAsync();
        return new ApiResponse(200);

    }

    public async Task<ApiResponse> UpdateReviewAsync(
        Guid busnissId,
        ReviewRequest request,
        Guid id,
        Guid userId)
    {

        var review = await _context.Reviews.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id && r.BusnissId == busnissId);
        if (review is null)
        {
            return new ApiResponse(StatusCodes.Status404NotFound, "Review Was Not Found");
        }
        if (review.UserId != userId)
        {
            return new ApiResponse(StatusCodes.Status401Unauthorized);
        }
        review.Rate = request.Rate;
        review.Comment = request.Comment;
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
        return new ApiResponse(StatusCodes.Status204NoContent);
    }

    public async Task<ApiResponse> DeleteAsync(Guid id, Guid ownerId)
    {
        var busniss = await _context.Busnisses.FirstOrDefaultAsync(b => b.Id == id);
        ItemNotFoundException.ThrowIfNull(busniss, nameof(busniss));

        if (busniss.UserId != ownerId)
        {
            return new ApiResponse(401);
        }

        _context.Busnisses.Remove(busniss);
        await _context.SaveChangesAsync();
        return new ApiResponse(200);
    }

    public async Task<ApiResponse> UpdateAsync(BusnisUpdateRequest request, Guid ownerId)
    {
        var busniss = await _context.Busnisses
            .Include(b => b.Location)
            .Include(b => b.ContactInfo)
            .FirstOrDefaultAsync(b => b.Id == request.id);

        ItemNotFoundException.ThrowIfNull(busniss, nameof(busniss));

        if (busniss.UserId != ownerId)
        {
            return new ApiResponse(401);
        }

        busniss.NameAR = request.Name;
        busniss.AboutAR = request.About;

        busniss.Location!.Latitude = request.Location.Latitude;
        busniss.Location.Longitude = request.Location.Longitude;
        busniss.Location.Address = request.Location.Address;

        busniss.ContactInfo!.FirstPhoneNumber = request.ContactInfo.FirstPhoneNumber;
        busniss.ContactInfo.SecoundPhoneNumber = request.ContactInfo.SecoundPhoneNumber;
        busniss.ContactInfo.ThirdPhoneNumber = request.ContactInfo.ThirdPhoneNumber;
        busniss.ContactInfo.EmailAddress = request.ContactInfo.EmailAddress;
        busniss.ContactInfo.SiteUrl = request.ContactInfo.SiteUrl;



        _context.Busnisses.Update(busniss);

        await _context.SaveChangesAsync();

        var days = request.WorkTime.Select(wt => wt.Day).ToList();

        await _context.Entry(busniss).Collection(b => b.WorkTime).LoadAsync();

        foreach (var wt in busniss.WorkTime)
        {
            if (days.Contains(wt.Day))
            {
                wt.Start = request.WorkTime.First(wtd => wtd.Day == wt.Day).Strart;
                wt.End = request.WorkTime.First(wtd => wtd.Day == wt.Day).End;
                await _workTimeService.Update(wt);
            }
            else
            {
                await _workTimeService.Remove(wt);
            }
        }

        foreach (var wt in request.WorkTime)
        {
            var exsists = busniss.WorkTime.Any(bwt => bwt.Day == wt.Day);
            if (!exsists)
            {
                await _workTimeService.Add(wt, busniss.Id);
            }
        }

        return new ApiResponse(200);
    }

    public async Task<ApiResponse> SetProfilePictureAsync(IFormFile picture, Guid busnissId, Guid ownerId)
    {
        var busniss = await _context.Busnisses.FindAsync(busnissId);


        if (busniss is null)
        {
            return new ApiResponse(500, $"{busnissId} Invalid Business ID");
        }

        if (busniss.UserId != ownerId)
        {
            return new ApiResponse(StatusCodes.Status401Unauthorized);
        }

        if (busniss.ProfilePictureUrl is not null
            && File.Exists(busniss.ProfilePictureUrl))
        {
            File.Delete(busniss.ProfilePictureUrl);
        }

        // Create directory if not exists
        var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "busniss", busniss.NameAR + busnissId.ToString().Substring(0, 8));
        Directory.CreateDirectory(uploadDirectory);



        // Generate unique file name

        var fileName = busnissId.ToString().Substring(0, 8) + Path.GetRandomFileName();

        var filePath = await _fileService.StoreAsync(picture, uploadDirectory, fileName);

        busniss.ProfilePictureUrl = filePath;

        _context.Update(busniss);

        await _context.SaveChangesAsync();


        return new ApiResponse(StatusCodes.Status200OK);
    }

    public async Task<ApiResponse> AddBusnissAlbumAsync([ListSize(5, 1)] List<IFormFile> album, Guid busnissId, Guid ownerId)
    {
        var busniss = await _context.Busnisses.FindAsync(busnissId);

        if (busniss is null)
        {
            return new ApiResponse(StatusCodes.Status500InternalServerError,
                $"InValid ID: {busnissId}");
        }

        if (busniss.UserId != ownerId)
        {
            return new ApiResponse(StatusCodes.Status401Unauthorized);
        }

        var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "busniss", busniss.NameAR + busnissId.ToString().Substring(0, 8), "Album");
        Directory.CreateDirectory(uploadDirectory);

        var filesInAlbum = Directory.GetFiles(uploadDirectory).Length;
        if (filesInAlbum > 0)
        {
            if (filesInAlbum < 5)
            {
                album = album.Take(5 - filesInAlbum)
                   .ToList();
            }
            else
            {
                return new ApiResponse(StatusCodes.Status500InternalServerError,
                    "Busniss Album Is Full");
            }
        }



        foreach (var file in album)
        {
            var fileName = busnissId.ToString().Substring(0, 8) + Path.GetRandomFileName();

            await _fileService.StoreAsync(file, uploadDirectory, fileName);

        }

        busniss.AlbumUrl = uploadDirectory;
        await _context.SaveChangesAsync();
        return new ApiResponse(StatusCodes.Status200OK);
    }

    public async Task<ApiResponse> RemoveFromAlbumAsync(string url, Guid busnissId, Guid ownerId)
    {
        var busniss = await _context.Busnisses.FindAsync(busnissId);


        if (busniss is null)
        {
            return new ApiResponse(500, $"{busnissId} Invalid Business ID");
        }

        if (busniss.UserId != ownerId)
        {
            return new ApiResponse(StatusCodes.Status401Unauthorized);
        }
        if (string.IsNullOrEmpty(busniss.AlbumUrl))
        {
            return new ApiResponse(StatusCodes.Status500InternalServerError, "No Album Exsits ");
        }
        var fileName = Path.GetFileName(url);
        var actualFilePath = Directory.GetFiles(busniss.AlbumUrl)
            .Where(filePath => filePath.EndsWith(fileName)).FirstOrDefault();
        if (busniss.AlbumUrl is not null
            && actualFilePath is not null
            && File.Exists(actualFilePath))
        {
            File.Delete(actualFilePath);
        }
        else
        {
            return new ApiResponse(StatusCodes.Status404NotFound, "file was not found");
        }

        return new ApiResponse(StatusCodes.Status204NoContent);
    }

    public async Task<ApiResponse> RemoveReviewAsync(Guid busnissId, Guid reviewId, Guid userId)
    {
        if (!await _context.Busnisses.AnyAsync(b => b.Id == busnissId))
        {
            return new ApiResponse(StatusCodes.Status404NotFound, "Busniss Not Found");
        }
        var review = await _context.Reviews
            .Where(r => r.Id == reviewId && r.BusnissId == busnissId)
            .FirstOrDefaultAsync();
        if (review is null)
        {
            return new ApiResponse(StatusCodes.Status404NotFound, "Review Not Found");
        }
        if (review.UserId != userId)
        {
            return new ApiResponse(StatusCodes.Status401Unauthorized);
        }
        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return new ApiResponse(StatusCodes.Status204NoContent);
    }


    #endregion

    #region Favorites
    public async Task<ApiResponse> AddToFavorites(Guid id, Guid userId)
    {
        if (!await _context.Busnisses.AsNoTracking().AnyAsync(b => b.Id == id))
        {
            return new ApiResponse(500, $"InValid Busniss ID:{id}");
        }

        await _favouriteService.AddToFavorites(id, userId);

        return new ApiResponse(200);
    }
    public async Task<List<FavoriteBusnissResponse>> GetFavorites(Guid userId)
    {
        var favIds = await _favouriteService.GetFavorites(userId);


        var favBusniss = await _context.Busnisses.AsNoTracking()
            .Where(b => favIds.Contains(b.Id))
            .Select(b => new FavoriteBusnissResponse
            {
                Name = b.NameAR,
                PictureUrl = b.ProfilePictureUrl,
                ReviewSummary = _reviewService.GetReviewSummary(b.Reviews),
                Category = b.Category!.Name
            })
            .ToListAsync();

        return favBusniss;
    }
    public async Task<ApiResponse> RemoveFromFavorites(Guid id, Guid userId)
    {
        if (!await _context.Busnisses.AnyAsync(b => b.Id == id))
        {
            return new ApiResponse(500, $"InValid Busniss ID:{id}");
        }
        await _favouriteService.RemoveFromFavorites(id, userId);
        return new ApiResponse(StatusCodes.Status204NoContent);
    }

    #endregion
}
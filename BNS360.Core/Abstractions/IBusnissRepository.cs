using BNS360.Core.Dtos.Request;
using BNS360.Core.Dtos.Response;
using BNS360.Core.Dtos.Response.AppBusniss;
using BNS360.Core.Entities;
using BNS360.Core.Errors;
using BNS360.Core.Helpers;
using Microsoft.AspNetCore.Http;

namespace BNS360.Core.Abstractions;

public interface IBusnissRepository
{
    Task<IReadOnlyList<BusnissReponse>> GetAllBusnissWithCategoryIdAsync(Guid Id);
    Task<IReadOnlyList<BusnissReponse>> GetRecommendedAsync();
    Task<ApiResponse> GetByIdAsync(Guid Id);
    Task<ApiResponse> CreateAsync(BusnisRequest request, Guid userId);
    Task<ApiResponse> Paganate(int pageNumber, int pageSize, Guid categoryId);
    Task<ApiResponse> UpdateAsync(BusnisUpdateRequest request, Guid ownerId);
    Task<ApiResponse> DeleteAsync(Guid id, Guid ownerId);
    Task<ApiResponse> UpdateReviewAsync(Guid busnissId,ReviewRequest request, Guid id,Guid userId);
    Task<ApiResponse> AddReviewAsync(ReviewRequest request, Guid userId);
    Task<ApiResponse> GetReviewsAsync(Guid id, int pageNumber, int size);
    Task<ApiResponse> RemoveReviewAsync(Guid busnissId, Guid reviewId, Guid userId);
    Task<List<FavoriteBusnissResponse>> GetFavorites(Guid userId);
    Task<ApiResponse> AddToFavorites(Guid id, Guid userId);
    Task<ApiResponse> RemoveFromFavorites(Guid id, Guid userId);
    Task<ApiResponse> FilterByDistance(Guid categoryId, CurrentLocation location, int pageNumber, int size);
    Task<ApiResponse> FilterByActive(Guid categoryId, int pageNUmber, int size);
    Task<ApiResponse> FilterByRatings(Guid categoryId, int pageNUmber, int size);
    Task<List<BusnissReponse>?> SearchAsync(string key, Guid categoryId);
    Task<ApiResponse> SetProfilePictureAsync(IFormFile picture, Guid busnissId, Guid ownerId);
    Task<ApiResponse> AddBusnissAlbumAsync([ListSize(maxSize:5,minSize:1)] List<IFormFile> album, Guid busnissId, Guid ownerId);

    Task<ApiResponse> RemoveFromAlbumAsync(string url,Guid busnissId , Guid ownerId);
}

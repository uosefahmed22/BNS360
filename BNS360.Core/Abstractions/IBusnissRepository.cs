using BNS360.Core.Dtos.Request;
using BNS360.Core.Dtos.Response.AppBusniss;
using BNS360.Core.Entities;
using BNS360.Core.Errors;

namespace BNS360.Core.Abstractions
{
    public interface IBusnissRepository
    {
        Task<IReadOnlyList<BusnissReponse>> GetAllBusnissWithCategoryIdAsync(Guid Id);
        Task<IReadOnlyList<BusnissReponse>> GetRecommendedAsync();
        Task<ApiResponse> GetByIdAsync(Guid Id);
        Task<ApiResponse> CreateAsync(BusnisRequest request, Guid userId);
    }
}

using BNS360.Core.Dto;
using BNS360.Core.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.IRepository
{
    public interface IBusinessRepository
    {
        Task<ApiResponse> CreateBusiness(BusinessModelDto model);
        Task<ApiResponse> UpdateBusiness(int businessId, BusinessModelDto model);
        Task<ApiResponse> DeleteBusiness(int businessId, string Userid);
        Task<ApiResponse> GetBusiness(int businessId);
        Task<ApiResponse> GetBusinessesByCategoery(int categoryId);
        Task<ApiResponse> GetBusinessesForBusinessOwnerAsync(string businessOwnerId);
        Task<ApiResponse> GetTopFive();
    }
}

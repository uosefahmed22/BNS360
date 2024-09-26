using BNS360.Core.Dto;
using BNS360.Core.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.IRepository
{
    public interface ICategoryRepository
    {
        Task<ApiResponse> CreateCategory(CategoryModelDto model);
        Task<ApiResponse> UpdateCategory(int categoryId, CategoryModelDto model);
        Task<ApiResponse> DeleteCategory(int categoryId);
        Task<ApiResponse> GetCategory(int categoryId);
        Task<ApiResponse> GetCategories();
    }
}

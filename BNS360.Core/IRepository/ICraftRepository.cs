using BNS360.Core.Dto;
using BNS360.Core.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.IRepository
{
    public interface ICraftRepository
    {
        Task<ApiResponse> CreateCraft(CraftsModelDto model);
        Task<ApiResponse> UpdateCraft(int CraftId, CraftsModelDto model);
        Task<ApiResponse> DeleteCraft(int CraftId);
        Task<ApiResponse> GetCrafts();
        Task<ApiResponse> GetCraft(int CraftId);
    }
}

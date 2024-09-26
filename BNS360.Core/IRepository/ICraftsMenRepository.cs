using BNS360.Core.Dto;
using BNS360.Core.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.IRepository
{
    public interface ICraftsMenRepository
    {
        Task <ApiResponse> Create(CraftsMenModelDto model);
        Task <ApiResponse> Update(int CraftsMenId, CraftsMenModelDto model);
        Task <ApiResponse> Delete(int CraftsMenId, string Userid);
        Task <ApiResponse> GetAll();
        Task <ApiResponse> GetByUserId(string UserId);
    }
}

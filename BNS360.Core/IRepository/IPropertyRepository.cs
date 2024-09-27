using BNS360.Core.Dto;
using BNS360.Core.Errors;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.IRepository
{
    public interface IPropertyRepository
    {
        Task<ApiResponse> AddProperty(PropertyModelDto model);
        Task<ApiResponse> DeleteProperty(int id, string UserId);
        Task<ApiResponse> UpdateProperty(int id, PropertyModelDto model);
        Task<ApiResponse> GetProperty(int id);
        Task<ApiResponse> GetProperties();
    }
}

using BNS360.Core.Dto;
using BNS360.Core.Errors;
using BNS360.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.IRepository
{
    public interface IFavoriteBusinessRepository
    {
        Task<ApiResponse> AddBusinessToFavorite(string userId, int businessId);
        Task<ApiResponse> RemoveBusinessFromFavorite(string userId, int businessId);
        Task<ApiResponse> GetFavoriteBusinesses(string userId);
        Task<ApiResponse> GetCraftsMenFavorites(string userId);
        Task<ApiResponse> RemovecraftsMenFromFavorite(string userId, int businessId);
        Task<ApiResponse> AddCraftsMenToFavorite(string userId, int businessId);
    }
}

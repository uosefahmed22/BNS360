using BNS360.Core.Entities;

namespace BNS360.Core.Services
{
    public interface IFavoriteService
    {
        Task AddToFavorites(Guid busnissId, Guid userId);
        Task RemoveFromFavorites(Guid busnissId, Guid userId);
        Task<HashSet<Guid>> GetFavorites(Guid userId);
    }
}

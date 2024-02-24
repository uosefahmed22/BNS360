using BNS360.Core.CustemExceptions;
using BNS360.Core.Entities;
using BNS360.Core.Services;
using BNS360.Reposatory.Data.AppBusniss;
using Microsoft.EntityFrameworkCore;

namespace BNS360.Reposatory.Repositories.Repositories
{
    public class FavoriteService : IFavoriteService
    {
        private readonly AppBusnissDbContext _context;

        public FavoriteService(AppBusnissDbContext context)
        {
            _context = context;
        }

        public async Task AddToFavorites(Guid busnissId, Guid userId)
        {
            if (await _context.FavoriteBusnisses.AnyAsync(fav => fav.BusnissId == busnissId && fav.UserId == userId))
            {
                throw new ItemExsists($"Busniss With The ID: {busnissId}, Already exsists in user {userId} Favorites ");
            }
            _context.FavoriteBusnisses.Add(new FavoriteBusniss { BusnissId = busnissId,UserId = userId});
            await _context.SaveChangesAsync();
        }

        public async Task<HashSet<Guid>> GetFavorites(Guid userId)
        {
            var favBusniss = await _context.FavoriteBusnisses.AsNoTracking()
                .Where(fav => fav.UserId == userId)
                .Select(fav => fav.BusnissId)
                .ToListAsync();

            if (favBusniss is null || !favBusniss.Any())
            {
                throw new ItemNotFoundException($"user {userId},has no favorite elements");
            }

            return favBusniss.ToHashSet();
        }

        public async Task RemoveFromFavorites(Guid busnissId, Guid userId)
        {

            var result = await _context.FavoriteBusnisses
                .Where(fav => fav.BusnissId == busnissId && fav.UserId == userId)
                .FirstOrDefaultAsync();
            if (result is null)
            {
                throw new ItemNotFoundException($"Busniss With The ID: {busnissId}, Dose not exsist in user {userId} Favorites ");
            }
            _context.FavoriteBusnisses.Remove(result);
            await _context.SaveChangesAsync();

        }
    }
}

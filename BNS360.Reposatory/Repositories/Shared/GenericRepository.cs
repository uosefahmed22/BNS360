using BNS360.Core.Entities;
using BNS360.Core.Services.Shared;
using BNS360.Core.Specifications;
using BNS360.Reposatory.Data.AppBusniss;
using BNS360.Reposatory.Data.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BNS360.Reposatory.Repositories.Shared
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly AppBusnissDbContext _context;

        public GenericRepository(AppBusnissDbContext context)
        {
            _context = context;
        }

        public async Task<T?> FindItemWithIdAsync(Guid id,bool includeNavigationPros = default)
        {
            var spc = new FindItemWithIdSpc<T>(id,includeNavigationPros);
            var item = await ApplayQuery(spc).FirstOrDefaultAsync();
            return item;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        private IQueryable<T> ApplayQuery(ISpecification<T> spc) =>
            SpecificationEvaluator<T>.Evaluate(_context.Set<T>().AsQueryable(), spc);
    }
}

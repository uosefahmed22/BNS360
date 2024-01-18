using BNS360.Core.Entities;
using BNS360.Core.Services.Shared;
using BNS360.Core.Specifications;
using BNS360.Reposatory.Data.Identity;
using Microsoft.EntityFrameworkCore;

namespace BNS360.Reposatory.Repositories.Shared
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly BNS360IdentityDbContext _context;

        public GenericRepository(BNS360IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<T?> FindItemWithId(int id,bool includeNavigationPros = false)
        {
            var spc = new FindItemWithIdSpc<T>(id,includeNavigationPros);
            var item = await ApplayQuery(spc).FirstOrDefaultAsync();
            return item;
        }
        private IQueryable<T> ApplayQuery(ISpecification<T> spc) =>
            SpecificationEvaluator<T>.Evaluate(_context.Set<T>().AsQueryable(), spc);
    }
}

using BNS360.Core.Entities;
using BNS360.Core.Services.Shared;

namespace BNS360.Reposatory.Repositories.Shared
{
    public static class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> Evaluate(IQueryable<TEntity> inputQuery
            , ISpecification<TEntity> spc)
        {
            var query = inputQuery;
            if (spc.Criteria is not null)
            {
                query = query.Where(spc.Criteria);
            }
            if (spc.OrderByExpression is not null)
                query = query.OrderBy(spc.OrderByExpression);

            if(spc.PageIndex >= 0 && spc.PageSize != 0)
            {
                query =  query.Skip(spc.PageIndex * spc.PageSize).Take(spc.PageSize);
            }
            return query;
        }
    }
}

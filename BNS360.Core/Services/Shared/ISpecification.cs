using BNS360.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Services.Shared
{
    public interface ISpecification<T> where T : BaseEntity
    {
        Expression<Func<T, bool>>? Criteria { get; }
        
        List<Expression<Func<T, object>>>? NavigationProperties { get; }

        Expression<Func<T, object>>? OrderByExpression { get; }

        int PageIndex { get; }
        int PageSize { get; }
    }
}

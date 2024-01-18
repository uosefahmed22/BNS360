using BNS360.Core.Entities;
using BNS360.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Services.Shared
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        //Task<T?> GetItem(Expression<Func<T,object>> critera);
        Task<T?> FindItemWithId(int id, bool includeNavigationPros = false);
    }
}

using BNS360.Core.Entities;
using System.Linq.Expressions;

namespace BNS360.Core.Specifications
{
    public class GetPageSpecification<T> : BaseSpecification<T> where T : BaseEntity 
    {
        public GetPageSpecification(int pageNumber,int Size,Expression<Func<T,bool>>? criteria = default)
        {
            PageIndex = pageNumber - 1;
            PageSize = Size;
            Criteria = criteria;
        }
    }
}

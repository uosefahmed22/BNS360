using BNS360.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Specifications
{
    public class FindItemWithIdSpc<T> :BaseSpecification<T> where T : BaseEntity
    {
        public FindItemWithIdSpc(int id, bool includeNavigationProps = false) 
        {
            Criteria = Item => Item.Id == id;
            IncludeNavigationProps = includeNavigationProps;
        }
    }
}

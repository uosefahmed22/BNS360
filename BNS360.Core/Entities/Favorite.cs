using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Entities
{
    public class Favorite
    {
        public Guid UserId { get; set; }
    }
    public class FavoriteBusniss : Favorite
    {
        public Guid BusnissId { get; set; }
    }

}

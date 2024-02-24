using BNS360.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BNS360.Reposatory.Data.config
{
    public class FavoriteBusnissConfigurations : IEntityTypeConfiguration<FavoriteBusniss>
    {
        public void Configure(EntityTypeBuilder<FavoriteBusniss> builder)
        {
           
            builder.HasKey(fav => new {fav.UserId,fav.BusnissId});
                
        }
    }
}

using BNS360.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BNS360.Reposatory.Data.config
{
    public class LocationConfigurations : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.Property(l => l.Address).IsRequired();
            builder.Property(l => l.Latitude).HasColumnType("decimal").IsRequired();
            builder.Property(l => l.Longitude).HasColumnType("decimal").IsRequired();
        }
    }    

}


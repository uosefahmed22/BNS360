using BNS360.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Repository.Data.Config
{
    public class PropertyConfiguration : IEntityTypeConfiguration<PropertyModel>
    {
        public void Configure(EntityTypeBuilder<PropertyModel> builder)
        {
            builder.Property(x => x.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Latitude).HasColumnType("decimal(18, 16)");
            builder.Property(x => x.Longitude).HasColumnType("decimal(18, 16)");
        }
    }
}

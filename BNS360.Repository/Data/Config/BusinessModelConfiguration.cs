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
    public class BusinessModelConfiguration : IEntityTypeConfiguration<BusinessModel>
    {
        public void Configure(EntityTypeBuilder<BusinessModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Latitude).HasColumnType("decimal(18, 16)");
            builder.Property(x => x.Longitude).HasColumnType("decimal(18, 16)");
            builder.Property(x => x.BusinessNameArabic).IsRequired();

            builder.HasOne(x => x.CategoryModel)
                .WithMany(x => x.BusinessModels)
                .HasForeignKey(x => x.CategoriesModelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

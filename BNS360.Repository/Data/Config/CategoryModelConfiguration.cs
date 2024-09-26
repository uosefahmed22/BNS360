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
    public class CategoryModelConfiguration : IEntityTypeConfiguration<CategoryModel>
    {
        public void Configure(EntityTypeBuilder<CategoryModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CategoryNameArabic).IsRequired();
            builder.HasMany(x => x.BusinessModels)
                .WithOne(x => x.CategoryModel)
                .HasForeignKey(x => x.CategoriesModelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

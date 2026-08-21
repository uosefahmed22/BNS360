using BNS360.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BNS360.Repository.Data.Config;

public sealed class FavoriteConfiguration : IEntityTypeConfiguration<FavoriteModel>
{
    public void Configure(EntityTypeBuilder<FavoriteModel> builder)
    {
        builder.HasIndex(x => new { x.UserId, x.businessId })
            .IsUnique()
            .HasFilter("[businessId] IS NOT NULL");

        builder.HasIndex(x => new { x.UserId, x.CraftsMenId })
            .IsUnique()
            .HasFilter("[CraftsMenId] IS NOT NULL");

        builder.ToTable(table => table.HasCheckConstraint(
            "CK_Favorites_ExactlyOneTarget",
            "([businessId] IS NOT NULL AND [CraftsMenId] IS NULL) OR ([businessId] IS NULL AND [CraftsMenId] IS NOT NULL)"));
    }
}

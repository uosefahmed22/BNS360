using BNS360.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BNS360.Repository.Data.Config;

public sealed class FeedbackConfiguration : IEntityTypeConfiguration<FeedbackModel>
{
    public void Configure(EntityTypeBuilder<FeedbackModel> builder)
    {
        builder.Property(x => x.Feedback).HasMaxLength(2_000).IsRequired();

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("CK_Feedbacks_Rating", "[rating] BETWEEN 1 AND 5");
            table.HasCheckConstraint(
                "CK_Feedbacks_ExactlyOneTarget",
                "([BusinessModelId] IS NOT NULL AND [CraftsMenModelId] IS NULL) OR ([BusinessModelId] IS NULL AND [CraftsMenModelId] IS NOT NULL)");
        });
    }
}

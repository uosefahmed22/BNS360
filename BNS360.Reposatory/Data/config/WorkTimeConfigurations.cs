using BNS360.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BNS360.Reposatory.Data.config
{
    public class WorkTimeConfigurations : IEntityTypeConfiguration<WorkTime>
    {
        public void Configure(EntityTypeBuilder<WorkTime> builder)
        {
            builder.HasOne<Busniss>()
                .WithMany(b => b.WorkTime)
                .HasForeignKey(wt => wt.BusnissId)
                .IsRequired(false);
        }
    }    

}


using BNS360.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Repository.Data.Config
{
    public class SavedJobsConfiguration : IEntityTypeConfiguration<SavedJobsModel>
    {
        public void Configure(EntityTypeBuilder<SavedJobsModel> builder)
        {
            builder.HasOne(s => s.JobModel)
                   .WithMany() 
                   .HasForeignKey(s => s.JobId)
                   .OnDelete(DeleteBehavior.NoAction); 

            builder.HasOne(s => s.User)
                   .WithMany()
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BNS360.Reposatory.Data.config
{
    public class RoleManagerConfig : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(new List<IdentityRole>
            {
                new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Default", NormalizedName = "DEFAULT" },
                new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "BusinessOwner", NormalizedName = "BUSINESSOWNER" },
                new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "ServiceProvider", NormalizedName = "SERVICEPROVIDER" },
            });
        }
    }
}

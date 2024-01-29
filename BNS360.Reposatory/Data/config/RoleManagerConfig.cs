using BNS360.Core.Helpers.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace BNS360.Reposatory.Data.config
{
    public class RoleManagerConfig : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
           
            builder.HasData(new List<IdentityRole>
            {
                new IdentityRole 
                { 
                    Id = Guid.NewGuid().ToString(),
                    Name = nameof(UserType.Default) ,
                    NormalizedName = nameof(UserType.Default).ToUpper()
                    ,ConcurrencyStamp = Guid.NewGuid().ToString() 
                },
                new IdentityRole
                { 
                    Id = Guid.NewGuid().ToString(),
                    Name = nameof(UserType.BusinssOwner) ,
                    NormalizedName = nameof(UserType.BusinssOwner).ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new IdentityRole 
                { 
                    Id = Guid.NewGuid().ToString(),
                    Name = nameof(UserType.ServiceProvider) ,
                    NormalizedName = nameof(UserType.ServiceProvider).ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
            });
        }
    }
}

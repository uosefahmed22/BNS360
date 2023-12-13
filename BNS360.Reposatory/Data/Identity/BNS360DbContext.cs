using BNS360.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BNS360.Reposatory.Data.Identity
{
    public class BNS360IdentityDbContext : IdentityDbContext
    {
        public BNS360IdentityDbContext(DbContextOptions<BNS360IdentityDbContext> options) : base(options) { }
        DbSet<AppUser> AppUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(BNS360IdentityDbContext).Assembly);
        }
    }
}
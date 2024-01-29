using BNS360.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BNS360.Reposatory.Data.AppBusniss
{
    public class AppBusnissDbContext : DbContext
    {
        public AppBusnissDbContext(DbContextOptions<AppBusnissDbContext> options)
            : base(options)
        {
            
        }
        public DbSet<Busniss> Busnisses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }  
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppBusnissDbContext).Assembly);
        }
    }
}
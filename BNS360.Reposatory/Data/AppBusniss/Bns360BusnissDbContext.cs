using BNS360.Core.Entities;
using BNS360.Core.Helpers;
using BNS360.Reposatory.Data.config;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BNS360.Reposatory.Data.AppBusniss
{
    public class AppBusnissDbContext : DbContext
    {
        public AppBusnissDbContext(DbContextOptions<AppBusnissDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Business> Busnisses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }  
        public DbSet<WorkTime> WorkTime { get; set; }
        public DbSet<FavoriteBusniss> FavoriteBusnisses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new WorkTimeConfigurations());
            modelBuilder.ApplyConfiguration(new LocationConfigurations());
            modelBuilder.ApplyConfiguration(new ReviewConfigurations());
            modelBuilder.ApplyConfiguration(new FavoriteBusnissConfigurations());

        }
    }
}
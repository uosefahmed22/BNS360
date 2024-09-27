using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BNS360.Core.Models.Auth;
using BNS360.Core.Models;

namespace BNS360.Repository.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }  
        public DbSet<BusinessModel> BusinessModels { get; set; }
        public DbSet<CategoryModel> CategoryModels { get; set; }
        public DbSet<CraftsModel> Crafts { get; set; }
        public DbSet<CraftsMenModel> CraftsMen { get; set; }
        public DbSet<FavoriteModel> Favorites { get; set; }
        public DbSet<FeedbackModel> Feedbacks { get; set; }
        public DbSet<JobModel> Jobs { get; set; }
        public DbSet<SavedJobsModel> SavedJobs { get; set; }
    }
}

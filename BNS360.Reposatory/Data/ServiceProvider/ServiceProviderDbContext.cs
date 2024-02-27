using BNS360.Reposatory.Data.ServiceProvider.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BNS360.Reposatory.Data.ServiceProvider
{
    public class ServiceProviderDbContext : DbContext
    {
        public ServiceProviderDbContext(DbContextOptions<ServiceProviderDbContext> options)
            :base(options) 
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServiceProviderDbContext).Assembly,
                t => t.IsSubclassOf(typeof(ServiceProviderConfigurations)));
            base.OnModelCreating(modelBuilder);
        }
    }
}

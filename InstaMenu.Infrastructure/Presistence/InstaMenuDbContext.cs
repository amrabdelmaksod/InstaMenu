using InstaMenu.Application.Interfaces;
using InstaMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Infrastructure.Presistence
{
    public class InstaMenuDbContext : DbContext, IInstaMenuDbContext
    {
        public InstaMenuDbContext(DbContextOptions<InstaMenuDbContext> options)
            : base(options)
        {
        }

        public DbSet<Merchant> Merchants => Set<Merchant>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<MenuItem> MenuItems => Set<MenuItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<MerchantSocialLink> MerchantSocialLinks => Set<MerchantSocialLink>();
        public DbSet<BusinessHour> BusinessHours => Set<BusinessHour>();
        public DbSet<MerchantSettings> MerchantSettings => Set<MerchantSettings>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(InstaMenuDbContext).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // This will be used by the design-time factory
                optionsBuilder.UseNpgsql();
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InstaMenu.Infrastructure.Presistence
{
    public class InstaMenuDbContextFactory : IDesignTimeDbContextFactory<InstaMenuDbContext>
    {
        public InstaMenuDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InstaMenuDbContext>();
            
            // For design-time, use environment variable or local.settings.json
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found in environment variables.");

            optionsBuilder.UseNpgsql(connectionString);

            return new InstaMenuDbContext(optionsBuilder.Options);
        }
    }
}
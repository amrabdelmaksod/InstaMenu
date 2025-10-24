using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InstaMenu.Infrastructure.Presistence
{
    public class InstaMenuDbContextFactory : IDesignTimeDbContextFactory<InstaMenuDbContext>
    {
        public InstaMenuDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InstaMenuDbContext>();
            
            // Build connection string from environment variables for design-time operations
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbUsername = Environment.GetEnvironmentVariable("DB_USERNAME");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            
            var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUsername};Password={dbPassword};SSL Mode=Require;Trust Server Certificate=true;";

            optionsBuilder.UseNpgsql(connectionString);

            return new InstaMenuDbContext(optionsBuilder.Options);
        }
    }
}
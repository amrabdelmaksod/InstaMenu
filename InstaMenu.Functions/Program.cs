using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Merchants.Queries;
using InstaMenu.Infrastructure.Presistence;
using InstaMenu.Infrastructure.Services;
using InstaMenuFunctions.Middlewares;
using InstaMenuFunctions.Configurations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;

// Create a host builder for Azure Functions
var builder = new HostBuilder();

// Configure the Functions Worker
builder.ConfigureFunctionsWorkerDefaults(workerBuilder =>
{
    // Add the CORS middleware
    workerBuilder.UseMiddleware<InstaMenuFunctions.Middlewares.CorsMiddleware>();
    // Add your JWT middleware
    workerBuilder.UseMiddleware<InstaMenuFunctions.Middlewares.JwtMiddleware>();
});

// Configure services
builder.ConfigureServices(services =>
{
    // Application Insights
    services.AddApplicationInsightsTelemetryWorkerService();
    services.ConfigureFunctionsApplicationInsights();

    // Database - Build connection string from environment variables
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
    var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
    var dbName = Environment.GetEnvironmentVariable("DB_NAME");
    var dbUsername = Environment.GetEnvironmentVariable("DB_USERNAME");
    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
    
    var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUsername};Password={dbPassword};SSL Mode=Require;Trust Server Certificate=true;";
    
    services.AddDbContext<InstaMenuDbContext>(options =>
        options.UseNpgsql(connectionString));

    // MediatR
    services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(GetMenuBySlugQueryHandler).Assembly);
    });

    // Services
    services.AddScoped<IWhatsAppService, TwilioWhatsAppService>();
    services.AddScoped<IInstaMenuDbContext, InstaMenuDbContext>();
    services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

    // OpenAPI Configuration
    services.AddSingleton<OpenApiConfigurationOptions>();
});

// Configure OpenAPI
builder.ConfigureOpenApi();

// Build and run
builder.Build().Run();

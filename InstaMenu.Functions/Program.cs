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

var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

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

    // Database
    var connectionString = Environment.GetEnvironmentVariable("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
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

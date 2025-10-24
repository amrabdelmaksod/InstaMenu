using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace InstaMenuFunctions.Configurations
{
    public class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
    {
        public override OpenApiInfo Info { get; set; } = new OpenApiInfo()
        {
            Version = "v1.0.0",
            Title = "InstaMenu API",
            Description = "A comprehensive API for managing restaurant menus, orders, and merchant operations",
            Contact = new OpenApiContact()
            {
                Name = "InstaMenu Support",
                Email = "support@instamenu.com",
                Url = new Uri("https://instamenu.com/support")
            },
            License = new OpenApiLicense()
            {
                Name = "MIT License",
                Url = new Uri("https://opensource.org/licenses/MIT")
            }
        };

        public override List<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>()
        {
            new OpenApiServer() { Url = "https://localhost:7071", Description = "Local Development Server" },
            new OpenApiServer() { Url = "https://instamenu-api.azurewebsites.net", Description = "Production Server" }
        };

        public override OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;
    }
}
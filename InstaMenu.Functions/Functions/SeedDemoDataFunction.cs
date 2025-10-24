using InstaMenu.Application.Interfaces;
using InstaMenu.Domain.Entities;
using InstaMenu.Infrastructure.Presistence;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;

public class SeedDemoDataFunction
{
    private readonly IInstaMenuDbContext _db;

    public SeedDemoDataFunction(IInstaMenuDbContext db)
    {
        _db = db;
    }

    [Function("SeedDemoData")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "seed-demo")] HttpRequestData req,
        FunctionContext executionContext)
    {
        if (await _db.Merchants.AnyAsync())
        {
            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteStringAsync("Demo data already exists");
            return res;
        }

        var merchant = new Merchant
        {
            Id = Guid.NewGuid(),
            Name = "Super Chicken",
            PhoneNumber = "+201020688350",
            Slug = "super-chicken",
            LogoUrl = null,
            CreatedAt = DateTime.UtcNow,    
            
        };

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "البرجر",
            SortOrder = 1,
            Merchant = merchant,
            
        };

        var item = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "سوبر تشيكن ساندويتش",
            Description = "تشيكن كريسبي مع صوص خاص",
            Price = 28,
            IsAvailable = true,
            Category = category,
            
        };

        _db.Merchants.Add(merchant);
        _db.Categories.Add(category);
        _db.MenuItems.Add(item);
        await _db.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync($"Seeded demo data. MenuItemId: {item.Id}");
        return response;
    }
}

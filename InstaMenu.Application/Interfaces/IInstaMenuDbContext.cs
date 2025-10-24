using InstaMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Interfaces
{
    public interface IInstaMenuDbContext
    {
        DbSet<Category> Categories { get; }
        DbSet<MenuItem> MenuItems { get; }
        DbSet<Merchant> Merchants { get; }
        DbSet<Order> Orders { get; }
        DbSet<MerchantSocialLink> MerchantSocialLinks { get; }
        DbSet<BusinessHour> BusinessHours { get; }
        DbSet<MerchantSettings> MerchantSettings { get; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

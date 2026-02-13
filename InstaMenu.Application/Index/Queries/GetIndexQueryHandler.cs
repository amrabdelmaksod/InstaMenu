using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Index.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InstaMenu.Application.Index.Queries;

public class GetIndexQueryHandler : IRequestHandler<GetIndexQuery, IndexViewModel>
{
    private readonly IInstaMenuDbContext _db;

    public GetIndexQueryHandler(IInstaMenuDbContext db)
    {
        _db = db;
    }

    public async Task<IndexViewModel> Handle(GetIndexQuery request, CancellationToken cancellationToken)
    {
        var orders = await _db.Orders.ToListAsync(cancellationToken);
        var totalOrdersAmount = orders.Sum(o => o.TotalPrice);
        var ordersCount = orders.Count;

        var menuVisitsCount = await _db.MerchantSettings.SumAsync(s => s.MenuVisitsCount, cancellationToken);

        var itemCounts = new Dictionary<Guid, int>();
        foreach (var order in orders)
        {
            if (string.IsNullOrWhiteSpace(order.ItemsJson)) continue;
            try
            {
                var items = JsonSerializer.Deserialize<List<OrderItem>>(order.ItemsJson);
                if (items == null) continue;
                foreach (var it in items)
                {
                    if (itemCounts.ContainsKey(it.ItemId)) itemCounts[it.ItemId] += it.Quantity;
                    else itemCounts[it.ItemId] = it.Quantity;
                }
            }
            catch
            {
                // ignore malformed JSON
            }
        }

        var mostOrderedItems = new List<TopItemDto>();
        var topSections = new List<TopSectionDto>();

        if (itemCounts.Any())
        {
            var itemIds = itemCounts.Keys.ToList();
            var items = await _db.MenuItems
                .Where(i => itemIds.Contains(i.Id))
                .Include(i => i.Category)
                .ToListAsync(cancellationToken);

            mostOrderedItems = itemCounts
                .OrderByDescending(kv => kv.Value)
                .Take(5)
                .Select(kv =>
                {
                    var item = items.FirstOrDefault(i => i.Id == kv.Key);
                    return new TopItemDto
                    {
                        ItemId = kv.Key,
                        Name = item?.Name ?? "Unknown",
                        Count = kv.Value
                    };
                })
                .ToList();

            var categoryCounts = new Dictionary<Guid, int>();
            foreach (var kv in itemCounts)
            {
                var item = items.FirstOrDefault(i => i.Id == kv.Key);
                if (item == null) continue;
                var catId = item.CategoryId;
                if (categoryCounts.ContainsKey(catId)) categoryCounts[catId] += kv.Value;
                else categoryCounts[catId] = kv.Value;
            }

            if (categoryCounts.Any())
            {
                var categoryIds = categoryCounts.Keys.ToList();
                var categories = await _db.Categories
                    .Where(c => categoryIds.Contains(c.Id))
                    .ToListAsync(cancellationToken);

                topSections = categoryCounts
                    .OrderByDescending(kv => kv.Value)
                    .Take(5)
                    .Select(kv => new TopSectionDto
                    {
                        SectionId = kv.Key,
                        Name = categories.FirstOrDefault(c => c.Id == kv.Key)?.Name ?? "Unknown",
                        VisitsCount = kv.Value
                    })
                    .ToList();
            }
        }

        var mostVisitedItems = mostOrderedItems.Select(i => i with { }).ToList();

        return new IndexViewModel
        {
            TotalOrdersAmount = totalOrdersAmount,
            OrdersCount = ordersCount,
            MenuVisitsCount = menuVisitsCount,
            MostOrderedItems = mostOrderedItems,
            MostVisitedItems = mostVisitedItems,
            TopSections = topSections
        };
    }

    private record OrderItem
    {
        public Guid ItemId { get; init; }
        public int Quantity { get; init; }
    }
}

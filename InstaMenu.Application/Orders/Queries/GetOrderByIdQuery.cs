using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Orders.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InstaMenu.Application.Orders.Queries;

public class GetOrderByIdQuery : IRequest<GetOrderByIdResponse?>
{
    public Guid Id { get; set; }
}

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, GetOrderByIdResponse?>
{
    private readonly IInstaMenuDbContext _context;

    public GetOrderByIdQueryHandler(IInstaMenuDbContext context)
    {
        _context = context;
    }

    public async Task<GetOrderByIdResponse?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {


        var order = await _context.Orders
            .Include(o => o.Merchant)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order == null) return null;

        // Deserialize JSON to get items and quantities
        var itemsData = JsonSerializer.Deserialize<List<OrderItemsDto>>(order.ItemsJson)!;

        // Get full details of MenuItems
        var itemIds = itemsData.Select(i => i.ItemId).ToList();

        var menuItems = await _context.MenuItems
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, cancellationToken);

        foreach (var item in itemsData)
        {
            if (menuItems.TryGetValue(item.ItemId, out var fullItem))
            {
                item.Name = fullItem.Name;
                item.Price = fullItem.Price;
            }
        }

        return new GetOrderByIdResponse
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            CustomerAddress = order.CustomerAddress,
            TotalPrice = order.TotalPrice,
            MerchantName = order.Merchant.Name,
            MerchantLogo = order.Merchant.LogoUrl,
            Items = itemsData
        };
    }
}

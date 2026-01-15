using InstaMenu.Application.Interfaces;
using InstaMenu.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InstaMenu.Application.Orders.Commands
{
    public class CreateOrderCommand : IRequest<CreateOrderResult>
    {
        public string MerchantSlug { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string? CustomerAddress { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }
    public class CreateOrderResult
    {
        public Guid OrderId { get; set; }
        public decimal Total { get; set; }
    }
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
    {
        private readonly IInstaMenuDbContext _db;

        public CreateOrderCommandHandler(IInstaMenuDbContext db)
        {
            _db = db;
        }

        public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {



            var merchant = await _db.Merchants
                .FirstOrDefaultAsync(m => m.Slug == request.MerchantSlug, cancellationToken);

            if (merchant == null)
                throw new Exception("Merchant not found");

            var menuItems = await _db.MenuItems
                .Where(i => request.Items.Select(x => x.ItemId).Contains(i.Id))
                .ToListAsync(cancellationToken);

            if (menuItems.Count != request.Items.Count)
                throw new Exception("Some items not found");

            // حساب السعر
            var total = request.Items.Sum(x =>
            {
                var item = menuItems.First(i => i.Id == x.ItemId);
                return item.Price * x.Quantity;
            });

            var order = new Order
            {
                Id = Guid.NewGuid(),
                MerchantId = merchant.Id,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                CustomerAddress = request.CustomerAddress,
                TotalPrice = total,
                ItemsJson = JsonSerializer.Serialize(request.Items),
                CreatedAt = DateTime.UtcNow
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync(cancellationToken);

            return new CreateOrderResult
            {
                OrderId = order.Id,
                Total = total
            };
        }
    }
}
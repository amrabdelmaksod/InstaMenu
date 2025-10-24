using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InstaMenu.Application.Orders.Commands
{
    public class SendOrderToWhatsAppCommand : IRequest<Unit>
    {
        public Guid OrderId { get; set; }
    }

    public class SendOrderToWhatsAppCommandHandler : IRequestHandler<SendOrderToWhatsAppCommand,Unit>
    {
        private readonly IInstaMenuDbContext _context;
        private readonly IWhatsAppService _whatsApp;

        public SendOrderToWhatsAppCommandHandler(IInstaMenuDbContext context, IWhatsAppService whatsApp)
        {
            _context = context;
            _whatsApp = whatsApp;
        }

        public async Task<Unit> Handle(SendOrderToWhatsAppCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Include(o => o.Merchant)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
                throw new Exception("Order not found");

            if (order.SentToWhatsapp)
                return Unit.Value;

            var items = JsonSerializer.Deserialize<List<OrderItemDto>>(order.ItemsJson)!;

            var itemIds = items.Select(i => i.ItemId).ToList();

            var menuItems = await _context.MenuItems
                .Where(i => itemIds.Contains(i.Id))
                .ToDictionaryAsync(i => i.Id, cancellationToken);

            var message = $"📦 *طلب جديد من InstaMenu*\n" +
                          $"👤 {order.CustomerName}\n" +
                          $"📞 {order.CustomerPhone}\n" +
                          $"🏠 {order.CustomerAddress}\n" +
                          $"🍽️ الطلب:\n";

            foreach (var item in items)
            {
                var itemName = menuItems.TryGetValue(item.ItemId, out var menuItem)
                    ? menuItem.Name
                    : "صنف غير معروف";

                message += $"- {item.Quantity} x {itemName}\n";
            }

            message += $"\n💰 الإجمالي: {order.TotalPrice} EGP";


            await _whatsApp.SendMessageAsync(order.Merchant.PhoneNumber, message);

            order.SentToWhatsapp = true;
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Common.Results;
using InstaMenu.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace InstaMenu.Application.Orders.Commands
{
    public class CreateOrderCommand : IRequest<Result<CreateOrderResult>>
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

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<CreateOrderResult>>
    {
        private readonly IInstaMenuDbContext _db;

        public CreateOrderCommandHandler(IInstaMenuDbContext db)
        {
            _db = db;
        }

        public async Task<Result<CreateOrderResult>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.CustomerName))
                    return Result<CreateOrderResult>.Failure(ResultErrors.BadRequest.MissingRequiredFields("CustomerName"));

                if (string.IsNullOrWhiteSpace(request.CustomerPhone))
                    return Result<CreateOrderResult>.Failure(ResultErrors.BadRequest.MissingRequiredFields("CustomerPhone"));

                if (!request.Items.Any())
                    return Result<CreateOrderResult>.Failure(ResultErrors.BadRequest.InvalidData("Order must contain at least one item"));

                // Validate phone number format (basic validation)
                if (!IsValidPhoneNumber(request.CustomerPhone))
                    return Result<CreateOrderResult>.Failure(ResultErrors.Validation.InvalidPhoneNumber(request.CustomerPhone));

                // Validate quantities
                if (request.Items.Any(i => i.Quantity <= 0))
                    return Result<CreateOrderResult>.Failure(ResultErrors.Validation.InvalidValue("quantity", "must be greater than 0"));

                var merchant = await _db.Merchants
                    .FirstOrDefaultAsync(m => m.Slug == request.MerchantSlug, cancellationToken);

                if (merchant == null)
                    return Result<CreateOrderResult>.Failure(ResultErrors.NotFound.Resource("Merchant", request.MerchantSlug));

                // Check if merchant is active (if you have status field)
                // if (merchant.Status != MerchantStatus.Active)
                // return Result<CreateOrderResult>.Failure(ResultErrors.BusinessLogic.MerchantNotActive());

                var menuItems = await _db.MenuItems
                    .Where(i => request.Items.Select(x => x.ItemId).Contains(i.Id))
                    .ToListAsync(cancellationToken);

                if (menuItems.Count != request.Items.Count)
                    return Result<CreateOrderResult>.Failure(ResultErrors.NotFound.MenuItem());

                // Check if all items are available
                var unavailableItems = menuItems.Where(i => !i.IsAvailable).ToList();
                if (unavailableItems.Any())
                    return Result<CreateOrderResult>.Failure(ResultErrors.BusinessLogic.MenuItemNotAvailable());

                // Calculate total price
                var total = request.Items.Sum(orderItem =>
                {
                    var menuItem = menuItems.First(i => i.Id == orderItem.ItemId);
                    return menuItem.Price * orderItem.Quantity;
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

                var result = new CreateOrderResult
                {
                    OrderId = order.Id,
                    Total = total
                };

                return Result.Success(result);
            }
            catch (Exception ex)
            {
                return Result<CreateOrderResult>.Failure(ResultErrors.Server.DatabaseError());
            }
        }

        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            // Basic phone number validation - adjust pattern as needed
            var phonePattern = @"^[\+]?[1-9][\d]{7,14}$";
            return Regex.IsMatch(phoneNumber, phonePattern);
        }
    }
}
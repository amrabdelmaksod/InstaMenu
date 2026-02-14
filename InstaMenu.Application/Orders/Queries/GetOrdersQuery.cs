using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Merchants.DTOs;
using InstaMenu.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Orders.Queries;

public class GetOrdersQuery : IRequest<List<MerchantOrderDto>>
{
    public Guid MerchantId { get; set; }
    public OrderStatus? Status { get; set; }
}

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<MerchantOrderDto>>
{
    private readonly IInstaMenuDbContext _context;

    public GetOrdersQueryHandler(IInstaMenuDbContext context)
    {
        _context = context;
    }

    public async Task<List<MerchantOrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Orders.AsQueryable()
            .Where(o => o.MerchantId == request.MerchantId);

        if (request.Status.HasValue)
            query = query.Where(o => o.Status == request.Status.Value);

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new MerchantOrderDto
            {
                OrderId = o.Id,
                CustomerName = o.CustomerName,
                CustomerPhone = o.CustomerPhone,
                Address = o.CustomerAddress,
                Total = o.TotalPrice,
                Status = o.Status.ToString(),
                CreatedAt = o.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return orders;
    }
}

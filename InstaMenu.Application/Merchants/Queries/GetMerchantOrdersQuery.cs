using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Merchants.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Queries
{
    public class GetMerchantOrdersQuery : IRequest<List<MerchantOrderDto>>
    {
        public Guid MerchantId { get; set; }
    }
    public class GetMerchantOrdersQueryHandler : IRequestHandler<GetMerchantOrdersQuery, List<MerchantOrderDto>>
    {
        private readonly IInstaMenuDbContext _context;

        public GetMerchantOrdersQueryHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<List<MerchantOrderDto>> Handle(GetMerchantOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _context.Orders
                .Where(o => o.MerchantId == request.MerchantId)
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


}

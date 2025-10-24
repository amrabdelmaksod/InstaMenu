using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Auth.Queries
{
    public class GetCurrentMerchantQuery : IRequest<CurrentMerchantDto>
    {
        public Guid MerchantId { get; set; }
    }

    public class GetCurrentMerchantQueryHandler : IRequestHandler<GetCurrentMerchantQuery, CurrentMerchantDto>
    {
        private readonly IInstaMenuDbContext _context;

        public GetCurrentMerchantQueryHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<CurrentMerchantDto> Handle(GetCurrentMerchantQuery request, CancellationToken cancellationToken)
        {
            var merchant = await _context.Merchants
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.MerchantId, cancellationToken);

            if (merchant == null) throw new Exception("Merchant not found");

            return new CurrentMerchantDto
            {
                Id = merchant.Id,
                Name = merchant.Name,
                PhoneNumber = merchant.PhoneNumber,
                Slug = merchant.Slug,
                LogoUrl = merchant.LogoUrl
            };
        }
    }

    public class CurrentMerchantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? LogoUrl { get; set; }
    }
}

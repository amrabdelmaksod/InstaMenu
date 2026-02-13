using InstaMenu.Application.Interfaces;
using InstaMenu.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Commands
{
    public class CompleteMerchantSetupCommand : IRequest<bool>
    {
        public Guid MerchantId { get; set; }
        public string Slug { get; set; } = null!;
        public string WhatsAppNumber { get; set; } = null!;
        public string Currency { get; set; } = "EGP";
    }

    public class CompleteMerchantSetupCommandHandler : IRequestHandler<CompleteMerchantSetupCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public CompleteMerchantSetupCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CompleteMerchantSetupCommand request, CancellationToken cancellationToken)
        {
            var merchant = await _context.Merchants
                .Include(m => m.Settings)
                .FirstOrDefaultAsync(m => m.Id == request.MerchantId, cancellationToken);

            if (merchant == null)
                throw new Exception("Merchant not found");

            // Check if slug is already taken by another merchant
            var slugExists = await _context.Merchants
                .AnyAsync(m => m.Slug == request.Slug && m.Id != request.MerchantId, cancellationToken);

            if (slugExists)
                throw new Exception("Slug is already taken");

            // Update merchant slug
            merchant.Slug = request.Slug;
            merchant.UpdatedAt = DateTime.UtcNow;

            // Create or update merchant settings
            if (merchant.Settings == null)
            {
                merchant.Settings = new MerchantSettings
                {
                    Id = Guid.NewGuid(),
                    MerchantId = merchant.Id,
                    WhatsAppNumber = request.WhatsAppNumber,
                    Currency = request.Currency,
                    CreatedAt = DateTime.UtcNow
                };
                _context.MerchantSettings.Add(merchant.Settings);
            }
            else
            {
                merchant.Settings.WhatsAppNumber = request.WhatsAppNumber;
                merchant.Settings.Currency = request.Currency;
                merchant.Settings.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

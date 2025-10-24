using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Commands
{
    public class UpsertMerchantSocialLinkCommand : IRequest<bool>
    {
        public Guid MerchantId { get; set; }
        public string Platform { get; set; } = null!;
        public string Url { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }

    public class UpsertMerchantSocialLinkCommandHandler : IRequestHandler<UpsertMerchantSocialLinkCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public UpsertMerchantSocialLinkCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpsertMerchantSocialLinkCommand request, CancellationToken cancellationToken)
        {
            var existingLink = await _context.MerchantSocialLinks
                .FirstOrDefaultAsync(l => l.MerchantId == request.MerchantId
                    && l.Platform == request.Platform, cancellationToken);

            if (existingLink != null)
            {
                // Update existing
                existingLink.Url = request.Url;
                existingLink.DisplayOrder = request.DisplayOrder;
                existingLink.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Add new
                _context.MerchantSocialLinks.Add(new InstaMenu.Domain.Entities.MerchantSocialLink
                {
                    Id = Guid.NewGuid(),
                    MerchantId = request.MerchantId,
                    Platform = request.Platform,
                    Url = request.Url,
                    DisplayOrder = request.DisplayOrder,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
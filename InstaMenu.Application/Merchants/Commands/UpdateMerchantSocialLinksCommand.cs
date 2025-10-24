using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Commands
{
    public class UpdateMerchantSocialLinksCommand : IRequest<bool>
    {
        public Guid MerchantId { get; set; }
        public List<SocialLinkDto> SocialLinks { get; set; } = new();
    }

    public class SocialLinkDto
    {
        public string Platform { get; set; } = null!;
        public string Url { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }

    public class UpdateMerchantSocialLinksCommandHandler : IRequestHandler<UpdateMerchantSocialLinksCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public UpdateMerchantSocialLinksCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateMerchantSocialLinksCommand request, CancellationToken cancellationToken)
        {
            var merchant = await _context.Merchants
                .Include(m => m.SocialLinks)
                .FirstOrDefaultAsync(m => m.Id == request.MerchantId, cancellationToken);

            if (merchant == null)
                return false;

            // Remove existing links
            _context.MerchantSocialLinks.RemoveRange(merchant.SocialLinks);

            // Add new links
            foreach (var linkDto in request.SocialLinks)
            {
                merchant.SocialLinks.Add(new InstaMenu.Domain.Entities.MerchantSocialLink
                {
                    Id = Guid.NewGuid(),
                    MerchantId = request.MerchantId,
                    Platform = linkDto.Platform,
                    Url = linkDto.Url,
                    DisplayOrder = linkDto.DisplayOrder,
                    CreatedAt = DateTime.UtcNow
                });
            }

            merchant.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
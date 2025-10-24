using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Commands
{
    public class DeleteMerchantSocialLinkCommand : IRequest<bool>
    {
        public Guid MerchantId { get; set; }
        public string Platform { get; set; } = null!;
    }

    public class DeleteMerchantSocialLinkCommandHandler : IRequestHandler<DeleteMerchantSocialLinkCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public DeleteMerchantSocialLinkCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteMerchantSocialLinkCommand request, CancellationToken cancellationToken)
        {
            var link = await _context.MerchantSocialLinks
                .FirstOrDefaultAsync(l => l.MerchantId == request.MerchantId
                    && l.Platform == request.Platform, cancellationToken);

            if (link == null)
                return false;

            // Soft delete
            link.IsDeleted = true;
            link.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
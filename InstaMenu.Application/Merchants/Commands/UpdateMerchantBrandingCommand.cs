using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Commands
{
    public class UpdateMerchantBrandingCommand : IRequest<bool>
    {
        public Guid MerchantId { get; set; }
        public string? LogoUrl { get; set; }
        public string? CoverImageUrl { get; set; }
    }

    public class UpdateMerchantBrandingCommandHandler : IRequestHandler<UpdateMerchantBrandingCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public UpdateMerchantBrandingCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateMerchantBrandingCommand request, CancellationToken cancellationToken)
        {
            var settings = await _context.MerchantSettings
                .FirstOrDefaultAsync(s => s.MerchantId == request.MerchantId, cancellationToken);

            if (settings == null)
                return false;

            settings.LogoUrl = request.LogoUrl;
            settings.CoverImageUrl = request.CoverImageUrl;
            settings.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
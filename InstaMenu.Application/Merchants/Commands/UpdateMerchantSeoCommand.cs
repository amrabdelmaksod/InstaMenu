using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Commands
{
    public class UpdateMerchantSeoCommand : IRequest<bool>
    {
        public Guid MerchantId { get; set; }
        public string? SeoTitle { get; set; }
        public string? SeoTitleAr { get; set; }
        public string? SeoDescription { get; set; }
        public string? SeoDescriptionAr { get; set; }
    }

    public class UpdateMerchantSeoCommandHandler : IRequestHandler<UpdateMerchantSeoCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public UpdateMerchantSeoCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateMerchantSeoCommand request, CancellationToken cancellationToken)
        {
            var settings = await _context.MerchantSettings
                .FirstOrDefaultAsync(s => s.MerchantId == request.MerchantId, cancellationToken);

            if (settings == null)
                return false;

            settings.SeoTitle = request.SeoTitle;
            settings.SeoTitleAr = request.SeoTitleAr;
            settings.SeoDescription = request.SeoDescription;
            settings.SeoDescriptionAr = request.SeoDescriptionAr;
            settings.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
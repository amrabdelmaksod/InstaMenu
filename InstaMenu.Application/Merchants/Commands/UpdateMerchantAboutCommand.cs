using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Commands
{
    public class UpdateMerchantAboutCommand : IRequest<bool>
    {
        public Guid MerchantId { get; set; }
        public string? AboutUs { get; set; }
        public string? AboutUsAr { get; set; }
    }

    public class UpdateMerchantAboutCommandHandler : IRequestHandler<UpdateMerchantAboutCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public UpdateMerchantAboutCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateMerchantAboutCommand request, CancellationToken cancellationToken)
        {
            var settings = await _context.MerchantSettings
                .FirstOrDefaultAsync(s => s.MerchantId == request.MerchantId, cancellationToken);

            if (settings == null)
                return false;

            settings.AboutUs = request.AboutUs;
            settings.AboutUsAr = request.AboutUsAr;
            settings.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
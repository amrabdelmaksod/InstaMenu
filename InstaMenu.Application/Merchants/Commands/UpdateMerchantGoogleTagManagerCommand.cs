using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Commands
{
    public class UpdateMerchantGoogleTagManagerCommand : IRequest<bool>
    {
        public Guid MerchantId { get; set; }
        public string? GoogleTagManagerId { get; set; }
        public bool IsGoogleTagManagerEnabled { get; set; }
    }

    public class UpdateMerchantGoogleTagManagerCommandHandler : IRequestHandler<UpdateMerchantGoogleTagManagerCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public UpdateMerchantGoogleTagManagerCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateMerchantGoogleTagManagerCommand request, CancellationToken cancellationToken)
        {
            var settings = await _context.MerchantSettings
                .FirstOrDefaultAsync(s => s.MerchantId == request.MerchantId, cancellationToken);

            if (settings == null)
                return false;

            settings.GoogleTagManagerId = request.GoogleTagManagerId;
            settings.IsGoogleTagManagerEnabled = request.IsGoogleTagManagerEnabled;
            settings.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
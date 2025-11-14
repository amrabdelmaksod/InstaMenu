using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Commands
{
    public class UpdateMerchantAboutCommand : IRequest<Result>
    {
        public Guid MerchantId { get; set; }
        public string? AboutUs { get; set; }
        public string? AboutUsAr { get; set; }
    }

    public class UpdateMerchantAboutCommandHandler : IRequestHandler<UpdateMerchantAboutCommand, Result>
    {
        private readonly IInstaMenuDbContext _context;

        public UpdateMerchantAboutCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateMerchantAboutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var settings = await _context.MerchantSettings
                    .FirstOrDefaultAsync(s => s.MerchantId == request.MerchantId, cancellationToken);

                if (settings == null)
                    return Result.Failure(ResultErrors.NotFound.MerchantSettings(request.MerchantId));

                settings.AboutUs = request.AboutUs;
                settings.AboutUsAr = request.AboutUsAr;
                settings.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                // In production, you should log the exception details
                // _logger.LogError(ex, "Error updating merchant about information for {MerchantId}", request.MerchantId);
                return Result.Failure(ResultErrors.Server.DatabaseError());
            }
        }
    }
}
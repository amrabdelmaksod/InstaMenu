using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Commands
{
    public class UpsertBusinessHourCommand : IRequest<bool>
    {
        public Guid MerchantId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public bool IsClosed { get; set; }
    }

    public class UpsertBusinessHourCommandHandler : IRequestHandler<UpsertBusinessHourCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public UpsertBusinessHourCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpsertBusinessHourCommand request, CancellationToken cancellationToken)
        {
            var settings = await _context.MerchantSettings
                .Include(s => s.BusinessHours)
                .FirstOrDefaultAsync(s => s.MerchantId == request.MerchantId, cancellationToken);

            if (settings == null)
                return false;

            var existingHour = settings.BusinessHours
                .FirstOrDefault(h => h.DayOfWeek == request.DayOfWeek);

            if (existingHour != null)
            {
                // Update existing
                existingHour.OpenTime = request.OpenTime;
                existingHour.CloseTime = request.CloseTime;
                existingHour.IsClosed = request.IsClosed;
                existingHour.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Add new
                settings.BusinessHours.Add(new InstaMenu.Domain.Entities.BusinessHour
                {
                    Id = Guid.NewGuid(),
                    MerchantSettingsId = settings.Id,
                    DayOfWeek = request.DayOfWeek,
                    OpenTime = request.OpenTime,
                    CloseTime = request.CloseTime,
                    IsClosed = request.IsClosed,
                    CreatedAt = DateTime.UtcNow
                });
            }

            settings.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
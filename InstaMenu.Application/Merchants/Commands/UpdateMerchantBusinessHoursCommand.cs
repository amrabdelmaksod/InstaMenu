using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Commands
{
    public class UpdateMerchantBusinessHoursCommand : IRequest<bool>
    {
        public Guid MerchantId { get; set; }
        public List<BusinessHourDto> BusinessHours { get; set; } = new();
    }

    public class BusinessHourDto
    {
        public int DayOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public bool IsClosed { get; set; }
    }

    public class UpdateMerchantBusinessHoursCommandHandler : IRequestHandler<UpdateMerchantBusinessHoursCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public UpdateMerchantBusinessHoursCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateMerchantBusinessHoursCommand request, CancellationToken cancellationToken)
        {
            var settings = await _context.MerchantSettings
                .Include(s => s.BusinessHours)
                .FirstOrDefaultAsync(s => s.MerchantId == request.MerchantId, cancellationToken);

            if (settings == null)
                return false;

            // Remove existing hours
            _context.BusinessHours.RemoveRange(settings.BusinessHours);

            // Add new hours
            foreach (var hourDto in request.BusinessHours)
            {
                settings.BusinessHours.Add(new InstaMenu.Domain.Entities.BusinessHour
                {
                    Id = Guid.NewGuid(),
                    MerchantSettingsId = settings.Id,
                    DayOfWeek = hourDto.DayOfWeek,
                    OpenTime = hourDto.OpenTime,
                    CloseTime = hourDto.CloseTime,
                    IsClosed = hourDto.IsClosed,
                    CreatedAt = DateTime.UtcNow
                });
            }

            settings.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
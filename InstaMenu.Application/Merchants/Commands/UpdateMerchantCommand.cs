using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Commands
{
    public class UpdateMerchantCommand : IRequest<bool>
    {
        public Guid MerchantId { get; set; }
        public string Name { get; set; } = null!;
        public string? NameAr { get; set; }
        public string Slug { get; set; } = null!;
        public int Status { get; set; }
    }

    public class UpdateMerchantCommandHandler : IRequestHandler<UpdateMerchantCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public UpdateMerchantCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateMerchantCommand request, CancellationToken cancellationToken)
        {
            var merchant = await _context.Merchants
                .FirstOrDefaultAsync(m => m.Id == request.MerchantId, cancellationToken);

            if (merchant == null)
                return false;

            merchant.Name = request.Name;
            merchant.NameAr = request.NameAr;
            merchant.Slug = request.Slug;
            merchant.Status = request.Status;
            merchant.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Commands
{
    public class UpdateMerchantCommand : IRequest<Result>
    {
        public Guid MerchantId { get; set; }
        public string Name { get; set; } = null!;
        public string? NameAr { get; set; }
        public string Slug { get; set; } = null!;
        public int Status { get; set; }
    }

    public class UpdateMerchantCommandHandler : IRequestHandler<UpdateMerchantCommand, Result>
    {
        private readonly IInstaMenuDbContext _context;

        public UpdateMerchantCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateMerchantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var merchant = await _context.Merchants
                    .FirstOrDefaultAsync(m => m.Id == request.MerchantId, cancellationToken);

                if (merchant == null)
                    return Result.Failure(ResultErrors.NotFound.Merchant(request.MerchantId));

                // Check for duplicate slug (excluding current merchant)
                var slugExists = await _context.Merchants
                    .AnyAsync(m => m.Id != request.MerchantId && m.Slug == request.Slug, cancellationToken);

                if (slugExists)
                    return Result.Failure(ResultErrors.Conflict.SlugAlreadyExists(request.Slug));

                // Validate slug format
                if (!IsValidSlug(request.Slug))
                    return Result.Failure(ResultErrors.Validation.InvalidSlug(request.Slug));

                merchant.Name = request.Name;
                merchant.NameAr = request.NameAr;
                merchant.Slug = request.Slug;
                merchant.Status = request.Status;
                merchant.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ResultErrors.Server.DatabaseError());
            }
        }

        private static bool IsValidSlug(string slug)
        {
            return !string.IsNullOrWhiteSpace(slug) &&
                   slug.All(c => char.IsLower(c) || char.IsDigit(c) || c == '-') &&
                   !slug.StartsWith('-') &&
                   !slug.EndsWith('-');
        }
    }
}
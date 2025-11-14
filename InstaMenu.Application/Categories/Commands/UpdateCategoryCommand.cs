using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest<Result>
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public int SortOrder { get; set; }
    }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
    {
        private readonly IInstaMenuDbContext _context;

        public UpdateCategoryCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

                if (category == null)
                    return Result.Failure(ResultErrors.NotFound.Category(request.CategoryId));

                // Check for duplicate category name within same merchant (excluding current category)
                var duplicateExists = await _context.Categories
                    .AnyAsync(c => c.MerchantId == category.MerchantId &&
                                   c.Id != request.CategoryId &&
                                   c.Name.ToLower() == request.Name.ToLower(), cancellationToken);

                if (duplicateExists)
                    return Result.Failure(ResultErrors.Conflict.CategoryNameExists(request.Name));

                // Validate sort order
                if (request.SortOrder < 0)
                    return Result.Failure(ResultErrors.Validation.InvalidSortOrder(request.SortOrder));

                category.Name = request.Name;
                category.SortOrder = request.SortOrder;

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ResultErrors.Server.DatabaseError());
            }
        }
    }
}

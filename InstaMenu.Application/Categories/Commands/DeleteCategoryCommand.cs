using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest<Result>
    {
        public Guid CategoryId { get; set; }
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly IInstaMenuDbContext _context;

        public DeleteCategoryCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.MenuItems)
                    .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

                if (category == null)
                    return Result.Failure(ResultErrors.NotFound.Category(request.CategoryId));

                // Check if category has menu items
                if (category.MenuItems.Any())
                    return Result.Failure(ResultErrors.BusinessLogic.CannotDeleteCategoryWithItems());

                // Check if this is the last category for the merchant
                var categoryCount = await _context.Categories
                    .CountAsync(c => c.MerchantId == category.MerchantId, cancellationToken);

                if (categoryCount <= 1)
                    return Result.Failure(ResultErrors.BusinessLogic.CannotDeleteLastCategory());

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(ResultErrors.Server.DatabaseError());
            }
        }
    }
}

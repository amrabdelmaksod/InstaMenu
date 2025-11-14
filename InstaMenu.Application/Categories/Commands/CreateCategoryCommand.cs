using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Common.Results;
using InstaMenu.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<Result<Guid>>
    {
        public Guid MerchantId { get; set; }
        public string Name { get; set; } = null!;
        public int SortOrder { get; set; }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
    {
        private readonly IInstaMenuDbContext _context;

        public CreateCategoryCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if merchant exists
                var merchantExists = await _context.Merchants
                    .AnyAsync(m => m.Id == request.MerchantId, cancellationToken);

                if (!merchantExists)
                    return Result<Guid>.Failure(ResultErrors.NotFound.Merchant(request.MerchantId));

                // Check for duplicate category name within merchant
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.MerchantId == request.MerchantId &&
                                    c.Name.ToLower() == request.Name.ToLower(), cancellationToken);

                if (categoryExists)
                    return Result<Guid>.Failure(ResultErrors.Conflict.CategoryNameExists(request.Name));

                // Validate sort order
                if (request.SortOrder < 0)
                    return Result<Guid>.Failure(ResultErrors.Validation.InvalidSortOrder(request.SortOrder));

                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    MerchantId = request.MerchantId,
                    Name = request.Name,
                    SortOrder = request.SortOrder
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success(category.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ResultErrors.Server.DatabaseError());
            }
        }
    }
}

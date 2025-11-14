using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Common.Results;
using InstaMenu.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.MenuItems.Commands
{
    public class CreateMenuItemCommand : IRequest<Result<Guid>>
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Result<Guid>>
    {
        private readonly IInstaMenuDbContext _context;

        public CreateMenuItemCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

                if (category == null)
                    return Result<Guid>.Failure(ResultErrors.NotFound.Category(request.CategoryId));

                // Validate price
                if (request.Price <= 0)
                    return Result<Guid>.Failure(ResultErrors.Validation.InvalidPrice(request.Price));

                // Check for duplicate menu item name within category
                var itemExists = await _context.MenuItems
                    .AnyAsync(mi => mi.CategoryId == request.CategoryId &&
                                    mi.Name.ToLower() == request.Name.ToLower(), cancellationToken);

                if (itemExists)
                    return Result<Guid>.Failure(ResultErrors.Conflict.MenuItemNameExists(request.Name));

                var item = new MenuItem
                {
                    Id = Guid.NewGuid(),
                    CategoryId = request.CategoryId,
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    ImageUrl = request.ImageUrl,
                    IsAvailable = true,
                };

                _context.MenuItems.Add(item);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success(item.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ResultErrors.Server.DatabaseError());
            }
        }
    }
}

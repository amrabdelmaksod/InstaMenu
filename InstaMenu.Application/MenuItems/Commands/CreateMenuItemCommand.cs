using InstaMenu.Application.Interfaces;
using InstaMenu.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.MenuItems.Commands
{
    public class CreateMenuItemCommand : IRequest<Guid>
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int? PreparationTimeInMinutes { get; set; }
        public string? Calories { get; set; }
        public bool HasMultipleSizes { get; set; }
        public List<MenuItemSizeDto>? Sizes { get; set; }
    }

    public class MenuItemSizeDto
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Calories { get; set; }
    }

    public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Guid>
    {
        private readonly IInstaMenuDbContext _context;

        public CreateMenuItemCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

            if (category == null)
                throw new Exception("Category not found");

            var item = new MenuItem
            {
                Id = Guid.NewGuid(),
                CategoryId = request.CategoryId,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                ImageUrl = request.ImageUrl,
                IsAvailable = true,
                PreparationTimeInMinutes = request.PreparationTimeInMinutes,
                Calories = request.Calories,
                HasMultipleSizes = request.HasMultipleSizes
            };

            if (request.HasMultipleSizes && request.Sizes != null && request.Sizes.Any())
            {
                foreach (var sizeDto in request.Sizes)
                {
                    item.Sizes.Add(new MenuItemSize
                    {
                        Id = Guid.NewGuid(),
                        MenuItemId = item.Id,
                        Name = sizeDto.Name,
                        Price = sizeDto.Price,
                        Calories = sizeDto.Calories
                    });
                }
            }

            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            return item.Id;
        }
    }
}

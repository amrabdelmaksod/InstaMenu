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
               
            };

            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            return item.Id;
        }
    }
}

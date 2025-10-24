using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.MenuItems.Commands
{
    public class UpdateMenuItemCommand : IRequest<bool>
    {
        public Guid MenuItemId { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
    }

    public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public UpdateMenuItemCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.MenuItems
                .FirstOrDefaultAsync(i => i.Id == request.MenuItemId, cancellationToken);

            if (item == null)
                return false;

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

            if (!categoryExists)
                throw new Exception("Target category not found");

            item.Name = request.Name;
            item.Description = request.Description;
            item.Price = request.Price;
            item.ImageUrl = request.ImageUrl;
            item.CategoryId = request.CategoryId;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

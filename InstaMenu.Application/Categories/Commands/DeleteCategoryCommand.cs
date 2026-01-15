using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest<bool>
    {
        public Guid CategoryId { get; set; }
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public DeleteCategoryCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .Include(c => c.MenuItems)
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

            if (category == null)
                return false;

            if (category.MenuItems.Any())
                throw new InvalidOperationException("Cannot delete category with menu items");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}

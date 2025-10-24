using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest<bool>
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public int SortOrder { get; set; }
    }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public UpdateCategoryCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

            if (category == null)
                return false;

            category.Name = request.Name;
            category.SortOrder = request.SortOrder;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.MenuItems.Commands
{
    public class DeleteMenuItemCommand : IRequest<bool>
    {
        public Guid MenuItemId { get; set; }
    }

    public class DeleteMenuItemCommandHandler : IRequestHandler<DeleteMenuItemCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public DeleteMenuItemCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.MenuItems
                .FirstOrDefaultAsync(i => i.Id == request.MenuItemId, cancellationToken);

            if (item == null)
                return false;

            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

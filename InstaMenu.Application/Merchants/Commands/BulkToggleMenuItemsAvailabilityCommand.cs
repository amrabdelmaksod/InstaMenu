using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Commands
{
    public class BulkToggleMenuItemsAvailabilityCommand : IRequest<bool>
    {
        public List<MenuItemAvailabilityDto> Items { get; set; } = new();
    }


    public class BulkToggleMenuItemsAvailabilityCommandHandler : IRequestHandler<BulkToggleMenuItemsAvailabilityCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public BulkToggleMenuItemsAvailabilityCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(BulkToggleMenuItemsAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var itemIds = request.Items.Select(i => i.MenuItemId).ToList();

            var menuItems = await _context.MenuItems
                .Where(i => itemIds.Contains(i.Id))
                .ToListAsync(cancellationToken);

            if (menuItems.Count != request.Items.Count)
                return false; // بعض الأصناف مش موجودة

            foreach (var item in menuItems)
            {
                var matching = request.Items.First(i => i.MenuItemId == item.Id);
                item.IsAvailable = matching.IsAvailable;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public class MenuItemAvailabilityDto
    {
        public Guid MenuItemId { get; set; }
        public bool IsAvailable { get; set; }
    }
}

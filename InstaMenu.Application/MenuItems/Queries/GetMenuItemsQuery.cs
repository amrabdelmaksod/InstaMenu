using InstaMenu.Application.Interfaces;
using InstaMenu.Application.MenuItems.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.MenuItems.Queries;

public class GetMenuItemsQuery : IRequest<MenuItemsDto>
{
    public Guid MerchantId { get; set; }
    public Guid? CategoryId { get; set; }
}

public class GetMenuItemsQueryHandler : IRequestHandler<GetMenuItemsQuery, MenuItemsDto>
{
    private readonly IInstaMenuDbContext _db;

    public GetMenuItemsQueryHandler(IInstaMenuDbContext db)
    {
        _db = db;
    }

    public async Task<MenuItemsDto> Handle(GetMenuItemsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.MenuItems
            .Include(i => i.Sizes)
            .Include(i => i.Category)
            .AsQueryable();

        query = query.Where(i => i.Category.MerchantId == request.MerchantId);

        if (request.CategoryId.HasValue)
            query = query.Where(i => i.CategoryId == request.CategoryId.Value);

        var items = await query.OrderBy(i => i.Name).ToListAsync(cancellationToken);

        var dto = new MenuItemsDto
        {
            Items = items.Select(i => new MenuItemDto
            {
                Id = i.Id,
                CategoryId = i.CategoryId,
                Name = i.Name,
                Description = i.Description,
                Price = i.Price,
                ImageUrl = i.ImageUrl,
                IsAvailable = i.IsAvailable,
                PreparationTimeInMinutes = i.PreparationTimeInMinutes,
                Calories = i.Calories,
                HasMultipleSizes = i.HasMultipleSizes,
                Sizes = i.Sizes.Select(s => new MenuItemSizeDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Price = s.Price,
                    Calories = s.Calories
                }).ToList()
            }).ToList()
        };

        return dto;
    }
}

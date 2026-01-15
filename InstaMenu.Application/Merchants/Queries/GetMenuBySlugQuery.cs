using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Merchants.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Queries
{

    public class GetMenuBySlugQuery : IRequest<GetMenuBySlugResponse?>
    {   
        public string Slug { get; set; } = null!;
    }


public class GetMenuBySlugQueryHandler : IRequestHandler<GetMenuBySlugQuery, GetMenuBySlugResponse?>
    {
        private readonly IInstaMenuDbContext _context;

        public GetMenuBySlugQueryHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<GetMenuBySlugResponse?> Handle(GetMenuBySlugQuery request, CancellationToken cancellationToken)
        {
            var merchant = await _context.Merchants
                .Include(m => m.Categories)
                    .ThenInclude(c => c.MenuItems)
                .FirstOrDefaultAsync(m => m.Slug == request.Slug, cancellationToken);

            if (merchant == null) return null;

            return new GetMenuBySlugResponse
            {
                MerchantName = merchant.Name,
                LogoUrl = merchant.LogoUrl,
                Categories = merchant.Categories
                    .OrderBy(c => c.SortOrder)
                    .Select(c => new CategoryDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Items = c.MenuItems
                            .Where(i => i.IsAvailable)
                            .Select(i => new MenuItemDto
                            {
                                Id = i.Id,
                                Name = i.Name,
                                Description = i.Description,
                                Price = i.Price,
                                ImageUrl = i.ImageUrl,
                                IsAvailable = i.IsAvailable
                            }).ToList()
                    }).ToList()
            };
        }
    }


}

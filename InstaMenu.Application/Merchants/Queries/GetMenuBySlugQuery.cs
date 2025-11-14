using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Common.Results;
using InstaMenu.Application.Merchants.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Merchants.Queries
{
  public class GetMenuBySlugQuery : IRequest<Result<GetMenuBySlugResponse>>
    {   
        public string Slug { get; set; } = null!;
  }

    public class GetMenuBySlugQueryHandler : IRequestHandler<GetMenuBySlugQuery, Result<GetMenuBySlugResponse>>
    {
        private readonly IInstaMenuDbContext _context;

   public GetMenuBySlugQueryHandler(IInstaMenuDbContext context)
 {
_context = context;
       }

        public async Task<Result<GetMenuBySlugResponse>> Handle(GetMenuBySlugQuery request, CancellationToken cancellationToken)
     {
try
    {
    var merchant = await _context.Merchants
       .Include(m => m.Categories)
      .ThenInclude(c => c.MenuItems)
      .FirstOrDefaultAsync(m => m.Slug == request.Slug, cancellationToken);

     if (merchant == null) 
       return Result<GetMenuBySlugResponse>.Failure(ResultErrors.NotFound.Resource("Merchant", request.Slug));

    // Check if merchant is active (if you have status field)
      // if (merchant.Status != MerchantStatus.Active)
     //     return Result<GetMenuBySlugResponse>.Failure(ResultErrors.BusinessLogic.MerchantNotActive());

     var response = new GetMenuBySlugResponse
      {
   MerchantName = merchant.Name,
   LogoUrl = merchant.LogoUrl,
Categories = merchant.Categories
      .Where(c => c.MenuItems.Any(i => i.IsAvailable)) // Only include categories with available items
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

       return Result.Success(response);
   }
catch (Exception ex)
   {
    return Result<GetMenuBySlugResponse>.Failure(ResultErrors.Server.DatabaseError());
      }
    }
  }
}

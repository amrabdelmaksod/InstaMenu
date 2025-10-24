using InstaMenu.Application.Interfaces;
using InstaMenu.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<Guid>
    {
        public Guid MerchantId { get; set; }
        public string Name { get; set; } = null!;
        public int SortOrder { get; set; }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
    {
        private readonly IInstaMenuDbContext _context;

        public CreateCategoryCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var merchantExists = await _context.Merchants
                .AnyAsync(m => m.Id == request.MerchantId, cancellationToken);

            if (!merchantExists)
                throw new Exception("Merchant not found");

            var category = new Category
            {
                Id = Guid.NewGuid(),
                MerchantId = request.MerchantId,
                Name = request.Name,
                SortOrder = request.SortOrder
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return category.Id;
        }
    }
}

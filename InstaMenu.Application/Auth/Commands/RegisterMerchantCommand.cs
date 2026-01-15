using InstaMenu.Application.Helpers;
using InstaMenu.Application.Interfaces;
using InstaMenu.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Auth.Commands
{
    public class RegisterMerchantCommand : IRequest<RegisterMerchantResult>
    {
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? LogoUrl { get; set; }
    }


    public class RegisterMerchantCommandHandler : IRequestHandler<RegisterMerchantCommand, RegisterMerchantResult>
    {
        private readonly IInstaMenuDbContext _context;
        private readonly IJwtTokenGenerator _jwt;

        public RegisterMerchantCommandHandler(IInstaMenuDbContext context, IJwtTokenGenerator jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        public async Task<RegisterMerchantResult> Handle(RegisterMerchantCommand request, CancellationToken cancellationToken)
        {
            var exists = await _context.Merchants
                .AnyAsync(m => m.PhoneNumber == request.PhoneNumber || m.Slug == request.Slug, cancellationToken);

            if (exists)
                throw new Exception("Phone number or slug already exists");

            var merchant = new Merchant
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                Slug = request.Slug,
                LogoUrl = request.LogoUrl,
                PasswordHash = PasswordHasher.Hash(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Merchants.Add(merchant);
            await _context.SaveChangesAsync(cancellationToken);

            var token = _jwt.GenerateToken(merchant.Id, merchant.Name);

            return new RegisterMerchantResult
            {
                MerchantId = merchant.Id,
                Token = token
            };
        }
    }

    public class RegisterMerchantResult
    {
        public Guid MerchantId { get; set; }
        public string Token { get; set; } = null!;
    }
}

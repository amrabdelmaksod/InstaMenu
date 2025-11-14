using InstaMenu.Application.Helpers;
using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Common.Results;
using InstaMenu.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Auth.Commands
{
    public class RegisterMerchantCommand : IRequest<Result<RegisterMerchantResult>>
    {
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? LogoUrl { get; set; }
    }

    public class RegisterMerchantCommandHandler : IRequestHandler<RegisterMerchantCommand, Result<RegisterMerchantResult>>
    {
        private readonly IInstaMenuDbContext _context;
        private readonly IJwtTokenGenerator _jwt;

        public RegisterMerchantCommandHandler(IInstaMenuDbContext context, IJwtTokenGenerator jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        public async Task<Result<RegisterMerchantResult>> Handle(RegisterMerchantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check for existing phone number
                var phoneExists = await _context.Merchants
                    .AnyAsync(m => m.PhoneNumber == request.PhoneNumber, cancellationToken);

                if (phoneExists)
                    return Result<RegisterMerchantResult>.Failure(ResultErrors.Conflict.EmailAlreadyExists(request.PhoneNumber));

                // Check for existing slug
                var slugExists = await _context.Merchants
                    .AnyAsync(m => m.Slug == request.Slug, cancellationToken);

                if (slugExists)
                    return Result<RegisterMerchantResult>.Failure(ResultErrors.Conflict.SlugAlreadyExists(request.Slug));

                // Validate password strength (optional)
                if (request.Password.Length < 6)
                    return Result<RegisterMerchantResult>.Failure(ResultErrors.Validation.InvalidValue("password", "must be at least 6 characters long"));

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

                var result = new RegisterMerchantResult
                {
                    MerchantId = merchant.Id,
                    Token = token
                };

                return Result.Success(result);
            }
            catch (Exception ex)
            {
                return Result<RegisterMerchantResult>.Failure(ResultErrors.Server.DatabaseError());
            }
        }
    }

    public class RegisterMerchantResult
    {
        public Guid MerchantId { get; set; }
        public string Token { get; set; } = null!;
    }
}

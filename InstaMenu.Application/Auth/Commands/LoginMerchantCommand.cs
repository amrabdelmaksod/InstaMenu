using InstaMenu.Application.Helpers;
using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Auth.Commands
{
    public class LoginMerchantCommand : IRequest<string?> // return JWT
    {
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginMerchantCommandHandler : IRequestHandler<LoginMerchantCommand, string?>
    {
        private readonly IInstaMenuDbContext _context;
        private readonly IJwtTokenGenerator _jwt;

        public LoginMerchantCommandHandler(IInstaMenuDbContext context, IJwtTokenGenerator jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        public async Task<string?> Handle(LoginMerchantCommand request, CancellationToken cancellationToken)
        {
            var merchant = await _context.Merchants
                .FirstOrDefaultAsync(m => m.PhoneNumber == request.PhoneNumber, cancellationToken);


            if (merchant == null) return null;

            var hashed = PasswordHasher.Hash(request.Password);

            if (merchant.PasswordHash != hashed) return null;

            return _jwt.GenerateToken(merchant.Id, merchant.Name);
        }
    }
}

using InstaMenu.Application.Helpers;
using InstaMenu.Application.Interfaces;
using InstaMenu.Application.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstaMenu.Application.Auth.Commands
{
    public class LoginMerchantCommand : IRequest<Result<LoginMerchantResult>>
    {
  public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

 public class LoginMerchantCommandHandler : IRequestHandler<LoginMerchantCommand, Result<LoginMerchantResult>>
    {
        private readonly IInstaMenuDbContext _context;
   private readonly IJwtTokenGenerator _jwt;

        public LoginMerchantCommandHandler(IInstaMenuDbContext context, IJwtTokenGenerator jwt)
       {
     _context = context;
    _jwt = jwt;
        }

        public async Task<Result<LoginMerchantResult>> Handle(LoginMerchantCommand request, CancellationToken cancellationToken)
        {
  try
            {
    var merchant = await _context.Merchants
      .FirstOrDefaultAsync(m => m.PhoneNumber == request.PhoneNumber, cancellationToken);

 if (merchant == null) 
 return Result<LoginMerchantResult>.Failure(ResultErrors.BadRequest.InvalidCredentials());

      var hashed = PasswordHasher.Hash(request.Password);

      if (merchant.PasswordHash != hashed) 
     return Result<LoginMerchantResult>.Failure(ResultErrors.BadRequest.InvalidCredentials());

      // Check if merchant is active (if you have a status field)
     // if (merchant.Status == MerchantStatus.Inactive)
     //     return Result<LoginMerchantResult>.Failure(ResultErrors.Forbidden.InactiveAccount());

        var token = _jwt.GenerateToken(merchant.Id, merchant.Name);

   var result = new LoginMerchantResult
     {
     MerchantId = merchant.Id,
  Token = token,
      Name = merchant.Name
       };

      return Result.Success(result);
   }
            catch (Exception ex)
     {
        return Result<LoginMerchantResult>.Failure(ResultErrors.Server.DatabaseError());
       }
        }
   }

    public class LoginMerchantResult
    {
 public Guid MerchantId { get; set; }
      public string Token { get; set; } = null!;
     public string Name { get; set; } = null!;
    }
}

using FluentValidation;

namespace InstaMenu.Application.Merchants.Queries
{
    public class GetMerchantQRCodeQueryValidator : AbstractValidator<GetMerchantQRCodeQuery>
    {
        public GetMerchantQRCodeQueryValidator()
        {
            RuleFor(x => x.MerchantId)
                .NotEmpty()
                .WithMessage("Merchant ID is required");
        }
    }
}

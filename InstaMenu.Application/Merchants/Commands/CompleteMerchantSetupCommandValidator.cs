using FluentValidation;
using System.Text.RegularExpressions;

namespace InstaMenu.Application.Merchants.Commands
{
    public class CompleteMerchantSetupCommandValidator : AbstractValidator<CompleteMerchantSetupCommand>
    {
        public CompleteMerchantSetupCommandValidator()
        {
            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("Slug is required")
                .MinimumLength(3).WithMessage("Slug must be at least 3 characters long")
                .MaximumLength(50).WithMessage("Slug must not exceed 50 characters")
                .Matches(@"^[a-z0-9-]+$").WithMessage("Slug can only contain lowercase letters, numbers, and hyphens");

            RuleFor(x => x.WhatsAppNumber)
                .NotEmpty().WithMessage("WhatsApp number is required")
                .Must(BeValidEgyptianPhoneNumber).WithMessage("Invalid Egyptian phone number format. Must be a valid Egyptian mobile number");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required")
                .Equal("EGP").WithMessage("Only EGP currency is currently supported");
        }

        private bool BeValidEgyptianPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Egyptian phone number validation (starts with +20 or 20 or 0, followed by 10-11 digits)
            var cleanedNumber = phoneNumber.Replace(" ", "").Replace("-", "");
            
            return Regex.IsMatch(cleanedNumber, @"^(\+20|20|0)?1[0125]\d{8}$");
        }
    }
}


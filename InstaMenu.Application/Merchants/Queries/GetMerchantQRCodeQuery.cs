using InstaMenu.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QRCoder;

namespace InstaMenu.Application.Merchants.Queries
{
    public class GetMerchantQRCodeQuery : IRequest<MerchantQRCodeDto>
    {
        public Guid MerchantId { get; set; }
    }

    public class MerchantQRCodeDto
    {
        public string MenuUrl { get; set; } = null!;
        public string QRCodeBase64 { get; set; } = null!;
        public string QRCodeUrl { get; set; } = null!;
    }

    public class GetMerchantQRCodeQueryHandler : IRequestHandler<GetMerchantQRCodeQuery, MerchantQRCodeDto>
    {
        private readonly IInstaMenuDbContext _context;
        private readonly IConfiguration _configuration;

        public GetMerchantQRCodeQueryHandler(IInstaMenuDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<MerchantQRCodeDto> Handle(GetMerchantQRCodeQuery request, CancellationToken cancellationToken)
        {
            var merchant = await _context.Merchants
                .Include(m => m.Settings)
                .FirstOrDefaultAsync(m => m.Id == request.MerchantId, cancellationToken);

            if (merchant == null)
                throw new Exception("Merchant not found");

            if (string.IsNullOrEmpty(merchant.Slug))
                throw new Exception("Merchant slug is not set. Please complete merchant setup first.");

            // Get base URL from configuration, fallback to default
            string baseUrl = _configuration["AppSettings:BaseUrl"] ?? $"http://localhost:7174/api/instamenu";
            
            // Generate menu URL
            string menuUrl = $"{baseUrl}/{merchant.Slug}";

            // Check if QR code already exists
            if (merchant.Settings?.QRCodeUrl != null)
            {
                return new MerchantQRCodeDto
                {
                    MenuUrl = menuUrl,
                    QRCodeBase64 = merchant.Settings.QRCodeUrl,
                    QRCodeUrl = merchant.Settings.QRCodeUrl
                };
            }

            // Generate new QR code
            string qrCodeBase64 = GenerateQRCode(menuUrl);

            // Save QR code to database
            if (merchant.Settings != null)
            {
                merchant.Settings.QRCodeUrl = qrCodeBase64;
                merchant.Settings.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create settings if they don't exist
                merchant.Settings = new Domain.Entities.MerchantSettings
                {
                    Id = Guid.NewGuid(),
                    MerchantId = merchant.Id,
                    QRCodeUrl = qrCodeBase64,
                    CreatedAt = DateTime.UtcNow
                };
                _context.MerchantSettings.Add(merchant.Settings);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new MerchantQRCodeDto
            {
                MenuUrl = menuUrl,
                QRCodeBase64 = qrCodeBase64,
                QRCodeUrl = qrCodeBase64
            };
        }

        private string GenerateQRCode(string url)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            
            byte[] qrCodeBytes = qrCode.GetGraphic(20);
            return $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
        }
    }
}

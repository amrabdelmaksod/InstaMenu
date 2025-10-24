using InstaMenu.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace InstaMenu.Infrastructure.Services
{
    public class TwilioWhatsAppService : IWhatsAppService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromNumber;

        public TwilioWhatsAppService(IConfiguration config)
        {
            _accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID") 
                ?? throw new InvalidOperationException("TWILIO_ACCOUNT_SID environment variable is required");
            
            _authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN") 
                ?? throw new InvalidOperationException("TWILIO_AUTH_TOKEN environment variable is required");
            
            _fromNumber = Environment.GetEnvironmentVariable("TWILIO_FROM_NUMBER") 
                ?? "whatsapp:+14155238886"; // Default sandbox number
            
            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task SendMessageAsync(string toPhoneNumber, string message)
        {
            var to = new PhoneNumber($"whatsapp:{toPhoneNumber}");

            await MessageResource.CreateAsync(
                from: new PhoneNumber(_fromNumber),
                to: to,
                body: message
            );
        }
    }
}
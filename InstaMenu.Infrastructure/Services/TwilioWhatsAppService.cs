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
            _accountSid = config["Twilio:AccountSid"] ?? throw new ArgumentNullException("Twilio:AccountSid");
            _authToken = config["Twilio:AuthToken"] ?? throw new ArgumentNullException("Twilio:AuthToken");
            _fromNumber = config["Twilio:FromNumber"] ?? "whatsapp:+14155238886";
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
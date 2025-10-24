namespace InstaMenu.Application.Interfaces
{
    public interface IWhatsAppService
    {
        Task SendMessageAsync(string toPhoneNumber, string message);
    }

}

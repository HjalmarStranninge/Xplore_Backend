namespace CC_Backend.Services
{
    public interface IEmailService
    {
        Task<(bool success, string message)> SendEmailAsync(string token, string emailAdress, string userName);
    }
}

namespace CC_Backend.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string token);
    }
}

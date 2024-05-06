namespace CC_Backend.Services
{
    public interface IEmailService
    {
        Task<(bool sucess, string message)> SendEmailAsync(string token, string emailAdress, string userName);
    }
}

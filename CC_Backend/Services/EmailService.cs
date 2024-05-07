using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
namespace CC_Backend.Services
{
    public class EmailService : IEmailService
    {

        // https://ethereal.email/create för att göra temporär email, bör bytas ut till riktig email, google etc.

        private readonly SmtpClient _smtpClient;
        private readonly MimeMessage _emailMessage;
        private readonly IConfiguration _config;

        public EmailService( MimeMessage emailMessage, IConfiguration config)
        {

            _smtpClient = new SmtpClient();
            _emailMessage = emailMessage;
            _config = config;
        }

        public async Task<(bool success, string message)> SendEmailAsync(string token, string emailAdress, string userName)
        {
            try
            {
                _emailMessage.From.Add(new MailboxAddress("Leatha Leannon", "leatha.leannon@ethereal.email"));
                _emailMessage.To.Add(new MailboxAddress(userName, emailAdress));
                _emailMessage.Subject = "Reset Password";
                _emailMessage.Body = new TextPart("plain")
                {
                    Text = "Dear "+ userName +"\nHere is your password reset token:\n" + token
                };

                await _smtpClient.ConnectAsync(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
                await _smtpClient.AuthenticateAsync(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
                await _smtpClient.SendAsync(_emailMessage);
                await _smtpClient.DisconnectAsync(true);

                return (true, "Message was sent"); 
            }
            catch (Exception ex)
            {
                
                return (false,ex.ToString()) ; 
            }
        }





    }
}

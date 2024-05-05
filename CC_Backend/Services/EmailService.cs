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

        public EmailService( MimeMessage emailMessage)
        {

            _smtpClient = new SmtpClient();
            _emailMessage = emailMessage;
        }

        public async Task<bool> SendEmailAsync(string token)
        {
            try
            {
                _emailMessage.From.Add(new MailboxAddress("Nikko Daniel", "nikko.daniel12@ethereal.email"));
                _emailMessage.To.Add(new MailboxAddress("Nikko Daniel", "nikko.daniel12@ethereal.email"));
                _emailMessage.Subject = "Reset Password";
                _emailMessage.Body = new TextPart("plain")
                {
                    Text = "Here is you email reset token: " + token
                };

                await _smtpClient.ConnectAsync("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
                await _smtpClient.AuthenticateAsync("nikko.daniel12@ethereal.email", "4cZU81RfCZNxZ8PMs1");
                await _smtpClient.SendAsync(_emailMessage);
                await _smtpClient.DisconnectAsync(true);

                return true; 
            }
            catch (Exception ex)
            {
                
                return false; 
            }
        }





    }
}

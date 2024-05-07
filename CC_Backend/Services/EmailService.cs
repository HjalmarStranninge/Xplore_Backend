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

                await _smtpClient.ConnectAsync("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
                await _smtpClient.AuthenticateAsync("leatha.leannon@ethereal.email", "NHjDdqv86avwXJ8kWA");
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

using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace CC_Backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly MimeMessage _emailMessage;

        public EmailService(MimeMessage emailMessage)
        {

            _smtpClient = new SmtpClient();
            _emailMessage = emailMessage;

        }
        // Email service for sending the reset password
        public async Task<(bool success, string message)> SendEmailAsync(string token, string emailAdress, string userName)
        {
            try
            {
                _emailMessage.From.Add(new MailboxAddress("The Nature Explorer Team", "natureexplorerai@gmail.com"));
                _emailMessage.To.Add(new MailboxAddress(userName, emailAdress));
                _emailMessage.Subject = "Reset Password";
                _emailMessage.Body = new TextPart("plain")
                {
                    Text = "Dear " + userName + "\nHere is your password reset token:\n" + token
                };

                await _smtpClient.ConnectAsync(Environment.GetEnvironmentVariable("EMAIL_HOST"), 587, SecureSocketOptions.StartTls);
                await _smtpClient.AuthenticateAsync(Environment.GetEnvironmentVariable("EMAIL_USERNAME"), Environment.GetEnvironmentVariable("EMAIL_PASSWORD"));
                await _smtpClient.SendAsync(_emailMessage);
                await _smtpClient.DisconnectAsync(true);

                return (true, "Message was sent");
            }
            catch (Exception ex)
            {

                return (false, ex.ToString());
            }
        }
    }
}

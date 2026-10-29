using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ShoppingCartCustomerMVC.Services.BackgroundService
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendRegistrationAcknowledgementAsync(
            string customerName,
            string email)
        {
            var smtpHost =
                _configuration["Email:SmtpHost"];

            var smtpPort =
                int.Parse(_configuration["Email:SmtpPort"]!);

            var username =
                _configuration["Email:Username"];

            var password =
                _configuration["Email:Password"];

            var fromEmail =
                _configuration["Email:FromEmail"];

            var fromName =
                _configuration["Email:FromName"];

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    fromName,
                    fromEmail));

            message.To.Add(
                new MailboxAddress(
                    customerName,
                    email));

            message.Subject =
                "Welcome to Cartly!";

            var body = $"""
                <html>
                <body style="font-family: Arial, sans-serif;">

                    <h2>Welcome to Cartly, {customerName}!</h2>

                    <p>
                        Thank you for creating your Cartly account.
                    </p>

                    <p>
                        Your account has been successfully registered.
                    </p>

                    <p>
                        You can now log in and start shopping.
                    </p>

                    <br />

                    <p>
                        Happy Shopping!
                    </p>

                    <p>
                        <strong>Cartly Team</strong>
                    </p>

                </body>
                </html>
                """;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(
                smtpHost,
                smtpPort,
                SecureSocketOptions.StartTls);

            await smtpClient.AuthenticateAsync(
                username,
                password);

            await smtpClient.SendAsync(message);

            await smtpClient.DisconnectAsync(true);
        }
    }
}
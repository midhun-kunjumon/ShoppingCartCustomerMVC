namespace ShoppingCartCustomerMVC.Services.BackgroundService
{
    public class HangfireEmailJob
    {
        private readonly EmailService _emailService;

        public HangfireEmailJob(EmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendRegistrationAcknowledgement(
            string customerName,
            string email)
        {
            await _emailService.SendRegistrationAcknowledgementAsync(
                customerName,
                email);
        }
    }
}
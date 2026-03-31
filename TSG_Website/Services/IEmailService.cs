namespace TSG_Website.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string email, string firstName, string password);
        Task SendRegistrationConfirmationAsync(string email, string name);
        Task SendRegistrationRejectedAsync(string email, string name, string? reason);
    }
}
namespace CinemaProject.Services;

public interface IEmailSender
{
    Task SendPasswordResetLinkAsync(string email, string name, string resetUrl, CancellationToken cancellationToken = default);
}

using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace CinemaProject.Services;

public sealed class SmtpEmailSender(IOptions<SmtpEmailOptions> options) : IEmailSender
{
    private readonly SmtpEmailOptions _options = options.Value;

    public async Task SendPasswordResetLinkAsync(
        string email,
        string name,
        string resetUrl,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Host) || string.IsNullOrWhiteSpace(_options.FromAddress))
        {
            throw new InvalidOperationException("SMTP email settings have not been configured.");
        }

        var safeName = WebUtility.HtmlEncode(name);
        var safeUrl = WebUtility.HtmlEncode(resetUrl);
        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromAddress, _options.FromName),
            Subject = "Reset your CineVerse password",
            IsBodyHtml = true,
            Body = $"<p>Hello {safeName},</p><p>Use the link below to choose a new password. This link expires in 30 minutes and can only be used once.</p><p><a href=\"{safeUrl}\">Reset password</a></p><p>If you didn't request this, you can ignore this email.</p>"
        };
        message.To.Add(new MailAddress(email));
        message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
            $"Hello {name},\r\n\r\nReset your CineVerse password using this link (expires in 30 minutes):\r\n{resetUrl}\r\n\r\nIf you didn't request this, ignore this email.",
            null,
            "text/plain"));

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false
        };

        if (!string.IsNullOrWhiteSpace(_options.UserName))
        {
            client.Credentials = new NetworkCredential(_options.UserName, _options.Password);
        }

#pragma warning disable SYSLIB0014
        await client.SendMailAsync(message, cancellationToken);
#pragma warning restore SYSLIB0014
    }
}

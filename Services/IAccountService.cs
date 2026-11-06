using CinemaProject.Models;

namespace CinemaProject.Services;

public enum AccountRegistrationResult
{
    Created,
    EmailAlreadyRegistered
}

public sealed record PasswordResetLinkData(string Email, string Name, string Token);

public interface IAccountService
{
    Task<AccountRegistrationResult> RegisterAsync(string name, string email, string password, string? profileImage, CancellationToken cancellationToken = default);
    Task<CustomerAccount?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<PasswordResetLinkData?> CreatePasswordResetLinkAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default);
}

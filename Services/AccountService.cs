using CinemaProject.Models;
using CinemaProject.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CinemaProject.Services;

public sealed class AccountService(
    IUnitOfWork unitOfWork,
    IPasswordHasher<CustomerAccount> passwordHasher) : IAccountService
{
    public async Task<AccountRegistrationResult> RegisterAsync(
        string name,
        string email,
        string password,
        string? profileImage,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        var alreadyExists = await unitOfWork.CustomerAccounts.QueryNoTracking()
            .AnyAsync(account => account.NormalizedEmail == normalizedEmail, cancellationToken);

        if (alreadyExists)
        {
            return AccountRegistrationResult.EmailAlreadyRegistered;
        }

        var account = new CustomerAccount
        {
            Name = name.Trim(),
            Email = email.Trim(),
            NormalizedEmail = normalizedEmail,
            ProfileImage = profileImage,
            CreatedUtc = DateTime.UtcNow
        };
        account.PasswordHash = passwordHasher.HashPassword(account, password);

        unitOfWork.CustomerAccounts.Add(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return AccountRegistrationResult.Created;
    }

    public async Task<CustomerAccount?> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        var account = await unitOfWork.CustomerAccounts.QueryNoTracking()
            .SingleOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);

        if (account is null)
        {
            return null;
        }

        var verification = passwordHasher.VerifyHashedPassword(account, account.PasswordHash, password);
        return verification is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded
            ? account
            : null;
    }

    public async Task<PasswordResetLinkData?> CreatePasswordResetLinkAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        var account = await unitOfWork.CustomerAccounts.Query()
            .SingleOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);
        if (account is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var previousTokens = await unitOfWork.PasswordResetTokens.Query()
            .Where(token => token.CustomerAccountId == account.Id && token.UsedUtc == null)
            .ToListAsync(cancellationToken);
        unitOfWork.PasswordResetTokens.RemoveRange(previousTokens);

        var token = CreateSecureToken();
        unitOfWork.PasswordResetTokens.Add(new PasswordResetToken
        {
            CustomerAccountId = account.Id,
            TokenHash = HashToken(token),
            CreatedUtc = now,
            ExpiresUtc = now.AddMinutes(30)
        });
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new PasswordResetLinkData(account.Email, account.Name, token);
    }

    public async Task<bool> ResetPasswordAsync(
        string token,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(token);
        var resetToken = await unitOfWork.PasswordResetTokens.Query()
            .Include(item => item.CustomerAccount)
            .SingleOrDefaultAsync(
                item => item.TokenHash == tokenHash && item.UsedUtc == null && item.ExpiresUtc > DateTime.UtcNow,
                cancellationToken);

        if (resetToken is null)
        {
            return false;
        }

        resetToken.CustomerAccount.PasswordHash = passwordHasher.HashPassword(resetToken.CustomerAccount, newPassword);
        resetToken.UsedUtc = DateTime.UtcNow;

        var otherTokens = await unitOfWork.PasswordResetTokens.Query()
            .Where(item => item.CustomerAccountId == resetToken.CustomerAccountId && item.Id != resetToken.Id && item.UsedUtc == null)
            .ToListAsync(cancellationToken);
        unitOfWork.PasswordResetTokens.RemoveRange(otherTokens);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string NormalizeEmail(string email) => email.Trim().ToUpperInvariant();

    private static string CreateSecureToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

    private static string HashToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}

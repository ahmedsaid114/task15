using System.ComponentModel.DataAnnotations;

namespace CinemaProject.Models;

public sealed class PasswordResetToken
{
    public int Id { get; set; }

    public int CustomerAccountId { get; set; }

    [Required, StringLength(64)]
    public string TokenHash { get; set; } = string.Empty;

    public DateTime CreatedUtc { get; set; }

    public DateTime ExpiresUtc { get; set; }

    public DateTime? UsedUtc { get; set; }

    public CustomerAccount CustomerAccount { get; set; } = null!;
}

using System.ComponentModel.DataAnnotations;

namespace CinemaProject.Models;

public sealed class CustomerAccount
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(256)]
    public string NormalizedEmail { get; set; } = string.Empty;

    [Required, StringLength(512)]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(400)]
    public string? ProfileImage { get; set; }

    public DateTime CreatedUtc { get; set; }
}

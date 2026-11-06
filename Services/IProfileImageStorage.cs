using Microsoft.AspNetCore.Http;

namespace CinemaProject.Services;

public interface IProfileImageStorage
{
    Task<bool> IsValidImageAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default);
    void Delete(string? imagePath);
}

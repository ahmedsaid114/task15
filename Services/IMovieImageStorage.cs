using Microsoft.AspNetCore.Http;

namespace CinemaProject.Services;

public interface IMovieImageStorage
{
    bool IsValidImage(IFormFile file);
    Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default);
    void Delete(string? imagePath);
}

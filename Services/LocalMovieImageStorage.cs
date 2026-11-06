using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CinemaProject.Services;

public sealed class LocalMovieImageStorage : IMovieImageStorage
{
    private const long MaximumImageSize = 5 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private readonly string _webRootPath;

    public LocalMovieImageStorage(IWebHostEnvironment environment)
    {
        _webRootPath = string.IsNullOrWhiteSpace(environment.WebRootPath)
            ? Path.Combine(environment.ContentRootPath, "wwwroot")
            : environment.WebRootPath;
    }

    public bool IsValidImage(IFormFile file) =>
        file.Length > 0
        && file.Length <= MaximumImageSize
        && AllowedExtensions.Contains(Path.GetExtension(file.FileName));

    public async Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid()}{extension}";
        var folder = Path.Combine(_webRootPath, "images", "movies");
        Directory.CreateDirectory(folder);

        await using var stream = new FileStream(
            Path.Combine(folder, fileName),
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);
        await file.CopyToAsync(stream, cancellationToken);

        return $"/images/movies/{fileName}";
    }

    public void Delete(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return;
        }

        var relativePath = imagePath.TrimStart('~', '/', '\\')
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);
        var root = Path.GetFullPath(_webRootPath) + Path.DirectorySeparatorChar;
        var fullPath = Path.GetFullPath(Path.Combine(_webRootPath, relativePath));

        if (fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase)
            && File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CinemaProject.Services;

public sealed class LocalProfileImageStorage : IProfileImageStorage
{
    private const long MaximumSize = 3 * 1024 * 1024;
    private readonly string _webRoot;

    public LocalProfileImageStorage(IWebHostEnvironment environment)
    {
        _webRoot = string.IsNullOrWhiteSpace(environment.WebRootPath)
            ? Path.Combine(environment.ContentRootPath, "wwwroot")
            : environment.WebRootPath;
    }

    public async Task<bool> IsValidImageAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file.Length is <= 0 or > MaximumSize)
        {
            return false;
        }

        var extension = Path.GetExtension(file.FileName);
        var header = new byte[12];
        await using var stream = file.OpenReadStream();
        var read = await stream.ReadAsync(header.AsMemory(), cancellationToken);

        return extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
            ? read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF
            : extension.Equals(".png", StringComparison.OrdinalIgnoreCase)
                ? read >= 8 && header.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })
                : extension.Equals(".webp", StringComparison.OrdinalIgnoreCase)
                    && read >= 12
                    && header.AsSpan(0, 4).SequenceEqual("RIFF"u8)
                    && header.AsSpan(8, 4).SequenceEqual("WEBP"u8);
    }

    public async Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var folder = Path.Combine(_webRoot, "images", "users");
        Directory.CreateDirectory(folder);

        await using var output = new FileStream(
            Path.Combine(folder, fileName), FileMode.CreateNew, FileAccess.Write, FileShare.None,
            bufferSize: 81920, useAsync: true);
        await file.CopyToAsync(output, cancellationToken);
        return $"/images/users/{fileName}";
    }

    public void Delete(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return;
        }

        var relative = imagePath.TrimStart('~', '/', '\\')
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);
        var root = Path.GetFullPath(_webRoot) + Path.DirectorySeparatorChar;
        var fullPath = Path.GetFullPath(Path.Combine(_webRoot, relative));
        if (fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase) && File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}

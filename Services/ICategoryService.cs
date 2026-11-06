using CinemaProject.Models;

namespace CinemaProject.Services;

public enum CategoryDeleteResult
{
    Deleted,
    NotFound,
    HasMovies
}

public interface ICategoryService
{
    Task<PagedResult<Category>> SearchAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreateAsync(Category category, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, Category category, CancellationToken cancellationToken = default);
    Task<CategoryDeleteResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

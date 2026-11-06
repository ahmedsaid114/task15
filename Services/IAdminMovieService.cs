using CinemaProject.Models;

namespace CinemaProject.Services;

public interface IAdminMovieService
{
    Task<PagedResult<Movie>> SearchAsync(string? search, int? categoryId, int? cinemaId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cinema>> GetCinemasAsync(CancellationToken cancellationToken = default);
    Task<Movie?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreateAsync(Movie movie, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, Movie movie, string? mainImagePath, CancellationToken cancellationToken = default);
    Task<(bool Found, string? ImagePath)> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

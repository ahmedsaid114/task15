using CinemaProject.ViewModels;

namespace CinemaProject.Services;

public interface IPublicMovieCatalog
{
    Task<CinemaHomeViewModel> GetHomeAsync(CancellationToken cancellationToken = default);
    Task<MovieListViewModel> SearchAsync(string? search, int? categoryId, CancellationToken cancellationToken = default);
    Task<MovieDetailsViewModel?> GetDetailsAsync(int id, CancellationToken cancellationToken = default);
}

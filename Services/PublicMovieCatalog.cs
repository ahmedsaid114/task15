using CinemaProject.Repositories;
using CinemaProject.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CinemaProject.Services;

public sealed class PublicMovieCatalog(IUnitOfWork unitOfWork) : IPublicMovieCatalog
{
    public async Task<CinemaHomeViewModel> GetHomeAsync(CancellationToken cancellationToken = default)
    {
        var movies = await unitOfWork.Movies.QueryNoTracking()
            .Where(movie => movie.Status)
            .OrderByDescending(movie => movie.ReleaseDate)
            .Take(4)
            .Select(movie => new MovieCardViewModel
            {
                Id = movie.Id,
                Name = movie.Name,
                Description = movie.Description,
                Price = movie.Price,
                ReleaseDate = movie.ReleaseDate,
                MainImg = movie.MainImg,
                CategoryName = movie.Category!.Name,
                CinemaName = movie.Cinema!.Name
            })
            .ToListAsync(cancellationToken);

        return new CinemaHomeViewModel
        {
            Movies = movies,
            Categories = await GetCategoriesAsync(cancellationToken)
        };
    }

    public async Task<MovieListViewModel> SearchAsync(
        string? search,
        int? categoryId,
        CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.Movies.QueryNoTracking()
            .Where(movie => movie.Status);

        if (categoryId.HasValue)
        {
            query = query.Where(movie => movie.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim();
            query = query.Where(movie => movie.Name.Contains(searchTerm));
        }

        var movies = await query
            .OrderByDescending(movie => movie.ReleaseDate)
            .Select(movie => new MovieCardViewModel
            {
                Id = movie.Id,
                Name = movie.Name,
                Description = movie.Description,
                Price = movie.Price,
                ReleaseDate = movie.ReleaseDate,
                MainImg = movie.MainImg,
                CategoryName = movie.Category!.Name,
                CinemaName = movie.Cinema!.Name
            })
            .ToListAsync(cancellationToken);

        return new MovieListViewModel
        {
            Movies = movies,
            Categories = await GetCategoriesAsync(cancellationToken),
            CategoryId = categoryId,
            Search = search
        };
    }

    public async Task<MovieDetailsViewModel?> GetDetailsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var movie = await unitOfWork.Movies.QueryNoTracking()
            .Where(item => item.Status && item.Id == id)
            .Select(item => new MovieDetailsViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                ReleaseDate = item.ReleaseDate,
                MainImg = item.MainImg,
                CategoryName = item.Category == null ? string.Empty : item.Category.Name,
                CinemaName = item.Cinema == null ? string.Empty : item.Cinema.Name,
                CinemaAddress = item.Cinema == null ? null : item.Cinema.Address,
                Actors = item.Actors.Select(actor => actor.Name).ToList(),
                Images = item.SubImages.Select(image => image.ImageUrl).ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);

        return movie;
    }

    private Task<List<CategoryViewModel>> GetCategoriesAsync(CancellationToken cancellationToken) =>
        unitOfWork.Categories.QueryNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name
            })
            .ToListAsync(cancellationToken);
}

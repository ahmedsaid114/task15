using CinemaProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Areas.Customer.Controllers;

[Area("Customer")]
public sealed class MoviesController : Controller
{
    private readonly IPublicMovieCatalog _movieCatalog;

    public MoviesController(IPublicMovieCatalog movieCatalog)
    {
        _movieCatalog = movieCatalog;
    }

    public async Task<IActionResult> Index(string? search, int? categoryId, CancellationToken cancellationToken)
    {
        var model = await _movieCatalog.SearchAsync(search, categoryId, cancellationToken);
        return View(model);
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var model = await _movieCatalog.GetDetailsAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }
}

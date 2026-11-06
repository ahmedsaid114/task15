using CinemaProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Areas.Customer.Controllers;

[Area("Customer")]
public sealed class HomeController : Controller
{
    private readonly IPublicMovieCatalog _movieCatalog;

    public HomeController(IPublicMovieCatalog movieCatalog)
    {
        _movieCatalog = movieCatalog;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = await _movieCatalog.GetHomeAsync(cancellationToken);
        return View(model);
    }
}

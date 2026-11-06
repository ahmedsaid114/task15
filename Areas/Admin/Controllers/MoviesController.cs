using CinemaProject.Models;
using CinemaProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaProject.Areas.Admin.Controllers;

[Area("Admin")]
public sealed class MoviesController : Controller
{
    private const int PageSize = 5;
    private readonly IAdminMovieService _movies;
    private readonly IMovieImageStorage _imageStorage;

    public MoviesController(IAdminMovieService movies, IMovieImageStorage imageStorage)
    {
        _movies = movies;
        _imageStorage = imageStorage;
    }

    public async Task<IActionResult> Index(string? searchString, int? categoryId, int? cinemaId, int page = 1, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        var result = await _movies.SearchAsync(searchString, categoryId, cinemaId, page, PageSize, cancellationToken);
        page = result.Page;

        await LoadDropdowns(categoryId, cinemaId, cancellationToken);
        ViewBag.SearchString = searchString?.Trim();
        ViewBag.CategoryId = categoryId;
        ViewBag.CinemaId = cinemaId;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = result.TotalPages;
        ViewBag.TotalMovies = result.TotalCount;
        return View(result.Items);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await LoadDropdowns(cancellationToken: cancellationToken);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Movie movie, IFormFile? imageFile, CancellationToken cancellationToken)
    {
        if (imageFile is null || imageFile.Length == 0)
        {
            ModelState.AddModelError("MainImg", "Please select a movie poster.");
        }
        else if (!_imageStorage.IsValidImage(imageFile))
        {
            ModelState.AddModelError("MainImg", "Please upload a valid JPG, PNG, or WEBP image under 5 MB.");
        }

        if (!ModelState.IsValid)
        {
            await LoadDropdowns(movie.CategoryId, movie.CinemaId, cancellationToken);
            return View(movie);
        }

        movie.MainImg = await _imageStorage.SaveAsync(imageFile!, cancellationToken);
        await _movies.CreateAsync(movie, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var movie = await _movies.GetByIdAsync(id, cancellationToken);
        if (movie is null)
        {
            return NotFound();
        }

        await LoadDropdowns(movie.CategoryId, movie.CinemaId, cancellationToken);
        return View(movie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Movie movie, IFormFile? imageFile, CancellationToken cancellationToken)
    {
        if (id != movie.Id)
        {
            return BadRequest();
        }

        var existingMovie = await _movies.GetByIdAsync(id, cancellationToken);
        if (existingMovie is null)
        {
            return NotFound();
        }

        if (imageFile is { Length: > 0 } && !_imageStorage.IsValidImage(imageFile))
        {
            ModelState.AddModelError("MainImg", "Please upload a valid JPG, PNG, or WEBP image under 5 MB.");
        }

        if (!ModelState.IsValid)
        {
            await LoadDropdowns(movie.CategoryId, movie.CinemaId, cancellationToken);
            return View(movie);
        }

        string? newImagePath = null;
        if (imageFile is { Length: > 0 })
        {
            newImagePath = await _imageStorage.SaveAsync(imageFile, cancellationToken);
        }

        var updated = await _movies.UpdateAsync(id, movie, newImagePath, cancellationToken);
        if (!updated)
        {
            _imageStorage.Delete(newImagePath);
            return NotFound();
        }

        if (newImagePath is not null)
        {
            _imageStorage.Delete(existingMovie.MainImg);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _movies.DeleteAsync(id, cancellationToken);
        if (!result.Found)
        {
            return NotFound();
        }

        _imageStorage.Delete(result.ImagePath);
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns(int? categoryId = null, int? cinemaId = null, CancellationToken cancellationToken = default)
    {
        var categories = await _movies.GetCategoriesAsync(cancellationToken);
        var cinemas = await _movies.GetCinemasAsync(cancellationToken);
        ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
        ViewBag.Cinemas = new SelectList(cinemas, "Id", "Name", cinemaId);
    }
}

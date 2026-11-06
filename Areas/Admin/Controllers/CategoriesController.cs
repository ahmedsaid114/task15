using CinemaProject.Models;
using CinemaProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Areas.Admin.Controllers;

[Area("Admin")]
public sealed class CategoriesController : Controller
{
    private const int PageSize = 5;
    private readonly ICategoryService _categories;

    public CategoriesController(ICategoryService categories)
    {
        _categories = categories;
    }

    public async Task<IActionResult> Index(string? searchString, int page = 1, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        var result = await _categories.SearchAsync(searchString, page, PageSize, cancellationToken);
        page = result.Page;

        ViewBag.SearchString = searchString?.Trim();
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = result.TotalPages;
        ViewBag.TotalCategories = result.TotalCount;
        return View(result.Items);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }

        await _categories.CreateAsync(category, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var category = await _categories.GetByIdAsync(id, cancellationToken);
        return category is null ? NotFound() : View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category, CancellationToken cancellationToken)
    {
        if (id != category.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(category);
        }

        var updated = await _categories.UpdateAsync(id, category, cancellationToken);
        return updated ? RedirectToAction(nameof(Index)) : NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _categories.DeleteAsync(id, cancellationToken);
        switch (result)
        {
            case CategoryDeleteResult.NotFound:
                return NotFound();
            case CategoryDeleteResult.HasMovies:
                TempData["ErrorMessage"] = "Cannot delete this category because it has movies.";
                break;
            case CategoryDeleteResult.Deleted:
                TempData["SuccessMessage"] = "Category deleted successfully.";
                break;
        }

        return RedirectToAction(nameof(Index));
    }
}

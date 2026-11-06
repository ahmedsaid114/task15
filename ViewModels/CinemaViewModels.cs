namespace CinemaProject.ViewModels;

// These are presentation models. Map your EF entities to them in the controller.
public sealed class CinemaHomeViewModel
{
    public IReadOnlyList<MovieCardViewModel> Movies { get; init; } = [];
    public IReadOnlyList<CategoryViewModel> Categories { get; init; } = [];
}

public sealed class MovieListViewModel
{
    public IReadOnlyList<MovieCardViewModel> Movies { get; init; } = [];
    public IReadOnlyList<CategoryViewModel> Categories { get; init; } = [];
    public int? CategoryId { get; init; }
    public string? Search { get; init; }
}

public sealed class MovieDetailsViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public DateTime ReleaseDate { get; init; }
    public string? MainImg { get; init; }
    public string CategoryName { get; init; } = "";
    public string CinemaName { get; init; } = "";
    public string? CinemaAddress { get; init; }
    public IReadOnlyList<string> Actors { get; init; } = [];
    public IReadOnlyList<string> Images { get; init; } = [];
}

public sealed class MovieCardViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public DateTime ReleaseDate { get; init; }
    public string? MainImg { get; init; }
    public string CategoryName { get; init; } = "";
    public string CinemaName { get; init; } = "";
}

public sealed class CategoryViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
}



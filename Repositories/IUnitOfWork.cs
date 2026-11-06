using CinemaProject.Models;

namespace CinemaProject.Repositories;

public interface IUnitOfWork
{
    IRepository<Movie> Movies { get; }
    IRepository<Category> Categories { get; }
    IRepository<Cinema> Cinemas { get; }
    IRepository<Actor> Actors { get; }
    IRepository<MovieSubImage> MovieSubImages { get; }
    IRepository<CustomerAccount> CustomerAccounts { get; }
    IRepository<PasswordResetToken> PasswordResetTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

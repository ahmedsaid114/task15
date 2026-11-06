using CinemaProject.Data;
using CinemaProject.Models;

namespace CinemaProject.Repositories;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public IRepository<Movie> Movies { get; } = new Repository<Movie>(context);
    public IRepository<Category> Categories { get; } = new Repository<Category>(context);
    public IRepository<Cinema> Cinemas { get; } = new Repository<Cinema>(context);
    public IRepository<Actor> Actors { get; } = new Repository<Actor>(context);
    public IRepository<MovieSubImage> MovieSubImages { get; } = new Repository<MovieSubImage>(context);
    public IRepository<CustomerAccount> CustomerAccounts { get; } = new Repository<CustomerAccount>(context);
    public IRepository<PasswordResetToken> PasswordResetTokens { get; } = new Repository<PasswordResetToken>(context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}

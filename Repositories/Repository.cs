using Microsoft.EntityFrameworkCore;

namespace CinemaProject.Repositories;

public sealed class Repository<TEntity>(DbContext context) : IRepository<TEntity>
    where TEntity : class
{
    private readonly DbSet<TEntity> _set = context.Set<TEntity>();

    public IQueryable<TEntity> Query() => _set;
    public IQueryable<TEntity> QueryNoTracking() => _set.AsNoTracking();
    public void Add(TEntity entity) => _set.Add(entity);
    public void Remove(TEntity entity) => _set.Remove(entity);
    public void RemoveRange(IEnumerable<TEntity> entities) => _set.RemoveRange(entities);
}

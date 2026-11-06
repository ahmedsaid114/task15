namespace CinemaProject.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query();
    IQueryable<TEntity> QueryNoTracking();
    void Add(TEntity entity);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
}

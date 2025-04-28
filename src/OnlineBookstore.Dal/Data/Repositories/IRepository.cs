using System.Linq.Expressions;
using OnlineBookstore.Dal.Entities;

namespace OnlineBookstore.Dal.Data.Repositories;

public interface IRepository<T> where T : class, IEntity
{
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate, 
        params Expression<Func<T, object>>[] includes);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
}

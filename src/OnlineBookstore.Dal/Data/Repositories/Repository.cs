using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Dal.Entities;

namespace OnlineBookstore.Dal.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class, IEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = ApplyIncludes(includes);
        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = ApplyIncludes(includes);
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = ApplyIncludes(includes);
        return await query.Where(predicate).ToListAsync();
    }

    private IQueryable<T> ApplyIncludes(Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
            query = query.Include(include);

        return query;
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
using BookingApi.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Repository.Common;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;    }
    

    public async Task DeleteAsync(Guid id)
    {
        //    await _dbSet.Where(e => e.Id == id).ExecuteDeleteAsync();
        var entity = await _dbSet.FindAsync(id);
        if (entity is null) return;
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
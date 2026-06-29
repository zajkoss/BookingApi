using BookingApi.Data;
using BookingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Repository;

public class ResourceRepository : IResourceRepository
{
    private readonly AppDbContext _dbContext;
    
    public ResourceRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<Resource>> GetAllAsync()
    {
        return await _dbContext.Resources.ToListAsync();
    }

    public async Task<Resource> CreateAsync(Resource resource)
    {
        await _dbContext.Resources.AddAsync(resource);
        await _dbContext.SaveChangesAsync();
        return resource;
    }

    public async Task<Resource?> GetByIdAsync(Guid id)
    {
        var firstOrDefaultAsync = await  _dbContext.Resources.FirstOrDefaultAsync(r => r.Id == id);
        return firstOrDefaultAsync;
    }

    public async Task<Resource?> UpdateAsync(Resource resource)
    {
        _dbContext.Resources.Update(resource);
        await _dbContext.SaveChangesAsync();
        return resource;
    }

}
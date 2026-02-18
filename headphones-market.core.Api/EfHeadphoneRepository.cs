using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Data;

public class EfHeadphoneRepository : IHeadphoneRepository
{
    private readonly HeadphonesDbContext _db;

    public EfHeadphoneRepository(HeadphonesDbContext db)
    {
        _db = db;
    }

    public async Task<List<Headphone>> GetAllAsync()
    {
        return await _db.Headphones.AsNoTracking().ToListAsync();
    }

    public async Task<Headphone?> GetByIdAsync(int id)
    {
        return await _db.Headphones.FindAsync(id);
    }

    public async Task<Headphone> AddAsync(Headphone item)
    {
        var entry = await _db.Headphones.AddAsync(item);
        await _db.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<bool> UpdateAsync(Headphone item)
    {
        var exists = await _db.Headphones.AsNoTracking().AnyAsync(h => h.Id == item.Id);
        if (!exists) return false;

        _db.Headphones.Update(item);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Headphones.FindAsync(id);
        if (entity is null) return false;
        _db.Headphones.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}
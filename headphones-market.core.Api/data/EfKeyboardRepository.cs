using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Data;

public class EfKeyboardRepository : IKeyboardRepository
{
    private readonly HeadphonesDbContext _db;

    public EfKeyboardRepository(HeadphonesDbContext db)
    {
        _db = db;
    }

    public async Task<List<Keyboard>> GetAllAsync()
        => await _db.Keyboards.AsNoTracking().ToListAsync();

    public async Task<Keyboard?> GetByIdAsync(int id)
        => await _db.Keyboards.FindAsync(id);

    public async Task<Keyboard> AddAsync(Keyboard item)
    {
        var entry = await _db.Keyboards.AddAsync(item);
        await _db.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<bool> UpdateAsync(Keyboard item)
    {
        var exists = await _db.Keyboards.AsNoTracking().AnyAsync(k => k.Id == item.Id);
        if (!exists) return false;
        _db.Keyboards.Update(item);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Keyboards.FindAsync(id);
        if (entity is null) return false;
        _db.Keyboards.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}
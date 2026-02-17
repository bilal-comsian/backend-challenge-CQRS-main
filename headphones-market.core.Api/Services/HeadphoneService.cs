using System.Collections.Generic;
using System.Threading.Tasks;
using headphones_market.core.Api.Data;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Services;

public class HeadphoneService : IHeadphoneService
{
    private readonly IHeadphoneRepository _repo;

    public HeadphoneService(IHeadphoneRepository repo)
    {
        _repo = repo;
    }

    // Queries
    public Task<List<Headphone>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Headphone?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

    // Commands
    public Task<Headphone> CreateAsync(Headphone create) => _repo.AddAsync(create);

    public async Task<bool> UpdateAsync(int id, Headphone update)
    {
        if (id != update.Id)
            return false;

        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return false;

        return await _repo.UpdateAsync(update);
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
}
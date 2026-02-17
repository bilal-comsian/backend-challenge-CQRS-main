using System.Collections.Generic;
using System.Threading.Tasks;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Services;

public interface IHeadphoneService
{
    // Queries
    Task<List<Headphone>> GetAllAsync();
    Task<Headphone?> GetByIdAsync(int id);

    // Commands
    Task<Headphone> CreateAsync(Headphone create);
    Task<bool> UpdateAsync(int id, Headphone update);
    Task<bool> DeleteAsync(int id);
}
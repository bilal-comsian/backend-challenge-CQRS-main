using System.Collections.Generic;
using System.Threading.Tasks;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Data;

public interface IHeadphoneRepository
{
    Task<List<Headphone>> GetAllAsync();
    Task<Headphone?> GetByIdAsync(int id);
    Task<Headphone> AddAsync(Headphone item);
    Task<bool> UpdateAsync(Headphone item);
    Task<bool> DeleteAsync(int id);
}
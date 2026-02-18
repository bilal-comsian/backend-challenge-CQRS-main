using System.Collections.Generic;
using System.Threading.Tasks;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Data;

public interface IKeyboardRepository
{
    Task<List<Keyboard>> GetAllAsync();
    Task<Keyboard?> GetByIdAsync(int id);
    Task<Keyboard> AddAsync(Keyboard item);
    Task<bool> UpdateAsync(Keyboard item);
    Task<bool> DeleteAsync(int id);
}
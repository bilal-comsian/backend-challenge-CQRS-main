using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using headphones_market.core.Api.Model;
using Newtonsoft.Json;

namespace headphones_market.core.Api.Data;

public class JsonHeadphoneRepository : IHeadphoneRepository
{
    private readonly string _filePath = "./data/headphones.json";
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

    public async Task<List<Headphone>> GetAllAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (!File.Exists(_filePath))
                return new List<Headphone>();

            var json = await File.ReadAllTextAsync(_filePath);
            var items = JsonConvert.DeserializeObject<List<Headphone>>(json) ?? new List<Headphone>();
            return items;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Headphone?> GetByIdAsync(int id)
    {
        var items = await GetAllAsync();
        return items.FirstOrDefault(x => x.Id == id);
    }

    public async Task<Headphone> AddAsync(Headphone item)
    {
        await _lock.WaitAsync();
        try
        {
            var items = await GetAllAsync();
            var nextId = items.Any() ? items.Max(x => x.Id) + 1 : 1;
            item.Id = nextId;
            items.Add(item);
            var json = JsonConvert.SerializeObject(items, Formatting.Indented);
            await File.WriteAllTextAsync(_filePath, json);
            return item;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> UpdateAsync(Headphone item)
    {
        await _lock.WaitAsync();
        try
        {
            var items = await GetAllAsync();
            var index = items.FindIndex(x => x.Id == item.Id);
            if (index < 0) return false;
            items[index] = item;
            var json = JsonConvert.SerializeObject(items, Formatting.Indented);
            await File.WriteAllTextAsync(_filePath, json);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await _lock.WaitAsync();
        try
        {
            var items = await GetAllAsync();
            var removed = items.RemoveAll(x => x.Id == id) > 0;
            if (!removed) return false;
            var json = JsonConvert.SerializeObject(items, Formatting.Indented);
            await File.WriteAllTextAsync(_filePath, json);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }
}
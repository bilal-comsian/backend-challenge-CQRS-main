using Microsoft.AspNetCore.Mvc;
using headphones_market.core.Api.Model;
using Newtonsoft.Json;

namespace headphones_market.core.Api.Endpoints;

[Route("[controller]")]
[ApiController]
public class HeadphonesController : ControllerBase
{
    public HeadphonesController()
    {
    }

    [HttpGet]
    public Task<List<Headphone>> Get()
    {
        List<Headphone> items = new List<Headphone>();
        using (StreamReader r = new StreamReader("./data/headphones.json"))
        {
            string json = r.ReadToEnd();
            items = JsonConvert.DeserializeObject<List<Headphone>>(json);
        }

        return Task.FromResult(items);
    }

    [HttpGet("{id}")]
    public Task<Headphone?> Get([FromRoute] int id)
    {
        List<Headphone> items = new List<Headphone>();
        using (StreamReader r = new StreamReader("./data/headphones.json"))
        {
            string json = r.ReadToEnd();
            items = JsonConvert.DeserializeObject<List<Headphone>>(json);
        }

        return Task.FromResult(items.FirstOrDefault(x => x.Id == id));
    }
}
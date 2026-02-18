namespace headphones_market.core.Api.Model;

public abstract class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Price { get; set; }
    public string ImageFileName { get; set; } = string.Empty;
    public bool Wireless { get; set; }
    public string Weight { get; set; } = string.Empty;
}
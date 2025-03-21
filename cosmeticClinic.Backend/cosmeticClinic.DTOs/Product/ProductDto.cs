namespace cosmeticClinic.DTOs.Product;

public class ProductDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Category { get; set; } = null!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Manufacturer { get; set; } = null!;
    public List<string> Ingredients { get; set; } = new();
    public string Usage { get; set; } = null!;
    public List<string> SideEffects { get; set; } = new();
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


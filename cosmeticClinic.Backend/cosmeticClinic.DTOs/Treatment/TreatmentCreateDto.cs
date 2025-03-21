namespace cosmeticClinic.DTOs.Product;

public class TreatmentCreateDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Category { get; set; } = null!;
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public List<string> RequiredEquipments { get; set; } = new();
    public List<string> PreRequisites { get; set; } = new();
    public List<string> AfterCare { get; set; } = new();
    public List<string> Risks { get; set; } = new();
    public string? ImageUrl { get; set; }
}
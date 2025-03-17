using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cosmeticClinic.Entities;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("description")]
    public string Description { get; set; } = null!;

    [BsonElement("category")]
    public ProductCategory Category { get; set; } 

    [BsonElement("price")]
    public decimal Price { get; set; }

    [BsonElement("stockQuantity")]
    public int StockQuantity { get; set; }

    [BsonElement("manufacturer")]
    public string Manufacturer { get; set; } = null!;

    [BsonElement("ingredients")]
    public List<string> Ingredients { get; set; } = new();

    [BsonElement("usage")]
    public string Usage { get; set; } = null!;

    [BsonElement("sideEffects")]
    public List<string> SideEffects { get; set; } = new();

    [BsonElement("imageUrl")]
    public string? ImageUrl { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("isDeleted")]
    public bool IsDeleted { get; set; } = false;
}

public enum ProductCategory
{
    Skincare,
    Haircare,
    Bodycare,
    SunProtection,
    AcneTreatment,
    Brightening,
    LipCare,
    EyeCare,
    PostTreatmentCare
}

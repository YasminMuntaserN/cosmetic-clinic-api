using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cosmeticClinic.Entities;

public class Treatment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("description")]
    public string Description { get; set; } = null!;

    [BsonElement("category")]
    public TreatmentCategory Category { get; set; } 

    [BsonElement("durationMinutes")]
    public int DurationMinutes { get; set; }

    [BsonElement("price")]
    public decimal Price { get; set; }

    [BsonElement("requiredEquipment")]
    public List<string> RequiredEquipment { get; set; } = new();

    [BsonElement("preRequisites")]
    public List<string> PreRequisites { get; set; } = new();

    [BsonElement("afterCare")]
    public List<string> AfterCare { get; set; } = new();

    [BsonElement("risks")]
    public List<string> Risks { get; set; } = new();

    [BsonElement("imageUrl")]
    public string? ImageUrl { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("isDeleted")]
    public bool IsDeleted { get; set; } = false;
}
public enum TreatmentCategory
{
    FacialTreatments,
    LaserTreatments,
    Injectables,
    BodyContouring,
    SkinRejuvenation,
    HairTreatments,
    AcneTreatments,
    AntiAgingTreatments,
    PigmentationTreatments,
    PostSurgeryCare
}
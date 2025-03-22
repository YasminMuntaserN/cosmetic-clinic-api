using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cosmeticClinic.Entities;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("email")]
    public string Email { get; set; } = null!;
    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = null!;
    [BsonElement("firstName")]
    public string FirstName { get; set; } = null!;
    [BsonElement("lastName")]
    public string LastName { get; set; } = null!;
    [BsonElement("role")]
    public string Role { get; set; } = null!;
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    [BsonElement("lastLogin")]
    public DateTime? LastLogin { get; set; }
    [BsonElement("isActive")]
    public bool IsActive { get; set; }
}
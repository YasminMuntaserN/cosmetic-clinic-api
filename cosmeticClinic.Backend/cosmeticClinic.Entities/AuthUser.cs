using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cosmeticClinic.Entities;

public class AuthUser
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string AuthUserId { get; set; } = null!;

    [BsonElement("UserId")]
    public string UserId { get; set; } = null!; 

    [BsonElement("refreshToken")]
    public string RefreshToken { get; set; } = null!;

    [BsonElement("refreshTokenExpiryTime")]
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
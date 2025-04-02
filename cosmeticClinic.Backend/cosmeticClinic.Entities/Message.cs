using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = null!;

    [BsonElement("content")]
    public string Content { get; set; } = null!;

    [BsonElement("senderId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string SenderId { get; set; } = null!; 

    [BsonElement("receiverId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ReceiverId { get; set; } = null!;  

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [BsonElement("isRead")]
    public bool IsRead { get; set; } = false;

}
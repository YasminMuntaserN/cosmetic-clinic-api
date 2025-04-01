using cosmeticClinic.Entities;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace cosmeticClinic.Settings.Hubs;

public class ChatHub : Hub
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<User> _collection;
    private readonly IMongoCollection<Message> _messageCollection;
    private static readonly Dictionary<string, string> _userConnections = new();

    public ChatHub(IMongoDatabase database)
    {
        _database = database;
        _collection =database.GetCollection<User>("users"); 
        _messageCollection =database.GetCollection<Message>("messages"); 
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();

        if (!string.IsNullOrEmpty(userId) && ObjectId.TryParse(userId, out var objectId))
        {
            _userConnections[userId] = Context.ConnectionId;

            var update = Builders<User>.Update
                .Set(u => u.Status, UserStatus.Online)
                .Set(u => u.LastSeen, DateTime.UtcNow);

            var filter = Builders<User>.Filter.Eq("_id", objectId);

            var result = await _collection.UpdateOneAsync(filter, update);

            await Clients.Others.SendAsync("UserConnected", userId);
        }
        else
        {
            Console.WriteLine("userId is null, empty, or not a valid ObjectId.");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString(); 

        if (!string.IsNullOrEmpty(userId) && ObjectId.TryParse(userId, out var objectId))
        {
            _userConnections.Remove(userId);

            var update = Builders<User>.Update
                .Set(u => u.Status, UserStatus.Offline)
                .Set(u => u.LastSeen, DateTime.UtcNow);
            
            var filter = Builders<User>.Filter.Eq("_id", objectId);
            
            var result = await _collection.UpdateOneAsync(filter, update);
            
            await Clients.Others.SendAsync("UserDisconnected", userId);
        }
        await base.OnDisconnectedAsync(exception);
    }

   public async Task SendMessage(string receiverId, string content)
{
    var senderId = Context.GetHttpContext()?.Request.Query["userId"].ToString(); 
    if (string.IsNullOrEmpty(senderId))
        throw new HubException("Unauthorized");
    
    if (string.IsNullOrEmpty(receiverId) || string.IsNullOrEmpty(content))
        throw new ArgumentException("Invalid message data");

    var message = new Message
    {
        SenderId = senderId,
        ReceiverId = receiverId,
        Content = content,
        Timestamp = DateTime.UtcNow,
        IsRead = false
    };

    try
    {
        await _messageCollection.InsertOneAsync(message);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Failed to insert message into database: {ex.Message}");
        throw;
    }

    if (_userConnections.TryGetValue(receiverId, out var connectionId))
    {
        await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
    }

    await Clients.Caller.SendAsync("MessageSent", message);
}

    public async Task MarkMessageAsRead(string messageId)
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString(); 
        if (string.IsNullOrEmpty(userId))
            throw new HubException("Unauthorized");

        var update = Builders<Message>.Update
            .Set(m => m.IsRead, true);

        await _messageCollection.UpdateOneAsync(
            m => m.Id == messageId && m.ReceiverId == userId,
            update
        );

        var message = await _messageCollection.Find(m => m.Id == messageId).FirstOrDefaultAsync();
        
        if (message != null && _userConnections.TryGetValue(message.SenderId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("MessageRead", messageId);
        }
    }
}
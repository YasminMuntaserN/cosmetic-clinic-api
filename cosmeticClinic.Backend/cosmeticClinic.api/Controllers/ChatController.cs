using AutoMapper;
using cosmeticClinic.Backend.Controllers.Base;
using cosmeticClinic.DTOs.Doctor;
using cosmeticClinic.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Manage Chat operations")]
public class ChatController : BaseController
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<ChatController> _logger;
    private readonly IMongoCollection<User> _userCollection;
    private readonly IMongoCollection<Message> _MessageCollection;
    private readonly IMapper _mapper;

    public ChatController(
        IMongoDatabase database,
        IMapper mapper,
        ILogger<ChatController> logger)
        : base(logger)
    {
        _mapper = mapper;
        _database = database;
        _userCollection = _database.GetCollection<User>("users");
        _MessageCollection = _database.GetCollection<Message>("messages");
        _logger = logger;
    }

    [HttpGet("messages/{userId}/{receiverId}")]
    [SwaggerOperation(Summary = "Retrieve messages between two users", 
        Description = "Fetches all chat messages exchanged between the specified users, sorted by timestamp.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the chat messages", typeof(IEnumerable<MessageDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No messages found between the specified users")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(string userId, string receiverId)
    {
        var messages = await _MessageCollection
            .Find(m =>
                (m.SenderId == userId && m.ReceiverId == receiverId) ||
                (m.SenderId == receiverId && m.ReceiverId == userId))
            .SortBy(m => m.Timestamp)
            .ToListAsync();

        if (messages == null || messages.Count == 0)
        {
            return NotFound(new { message = "No messages found between the specified users." });
        }

        return Ok(_mapper.Map<IEnumerable<MessageDto>>(messages));
    }

    [HttpGet("conversations/{userId}")]
    [SwaggerOperation(Summary = "Retrieve conversation history for a user")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns a list of conversations", typeof(IEnumerable<object>))]
    public async Task<IActionResult> GetConversations(string userId)
    {
        var messages = await _MessageCollection
            .Find(m => m.SenderId == userId || m.ReceiverId == userId)
            .ToListAsync();

        var groupedMessages = messages
            .GroupBy(m => new { ConversationKey = GetConversationKey(m.SenderId, m.ReceiverId) })
            .Select(g => new
            {
                OtherUserId = g.First().SenderId == userId ? g.First().ReceiverId : g.First().SenderId,
                LastMessage = g.OrderByDescending(m => m.Timestamp).First().Content,
                LastMessageTime = g.OrderByDescending(m => m.Timestamp).First().Timestamp,
                UnreadMessages = g.Count(m => m.ReceiverId == userId && !m.IsRead)
            })
            .ToList();

        var users = await _userCollection
            .Find(u => groupedMessages.Select(g => g.OtherUserId).Contains(u.Id.ToString()))
            .Project<User>(Builders<User>.Projection.Exclude(u => u.PasswordHash))
            .ToListAsync();

        var result = groupedMessages.Select(g => new
        {
            User = users.FirstOrDefault(u => u.Id.ToString() == g.OtherUserId),
            LastMessage = g.LastMessage,
            LastMessageTime = g.LastMessageTime,
            UnreadMessages = g.UnreadMessages
        })
        .OrderByDescending(r => r.LastMessageTime)
        .ToList();

        return Ok(result);
    }

    private string GetConversationKey(string senderId, string receiverId)
    {
        return string.Compare(senderId, receiverId) < 0
            ? $"{senderId}-{receiverId}"
            : $"{receiverId}-{senderId}";
    }
}

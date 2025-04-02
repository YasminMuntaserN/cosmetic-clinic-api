namespace cosmeticClinic.DTOs.Doctor;

public class MessageDto
{
    public string Id { get; set; }
    public string Content { get; set; }
    public string SenderId { get; set; } 
    public string ReceiverId { get; set; } 
    public DateTime Timestamp { get; set; }
    public bool IsRead { get; set; } 
}
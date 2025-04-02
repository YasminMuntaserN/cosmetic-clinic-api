namespace cosmeticClinic.DTOs.Doctor;

public class UserDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; }
    public string? Status { get; set; }
    public DateTime? LastSeen { get; set; }
}
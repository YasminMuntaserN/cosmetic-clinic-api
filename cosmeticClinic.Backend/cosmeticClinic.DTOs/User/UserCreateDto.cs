namespace cosmeticClinic.DTOs.Doctor;

public class UserCreateDto
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public DateTime? CreatedAt { get; set; } =DateTime.Now;
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; } = true;
}
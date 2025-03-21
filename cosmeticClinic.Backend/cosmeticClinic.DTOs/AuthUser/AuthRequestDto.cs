namespace cosmeticClinic.DTOs.Doctor;

public class AuthRequestDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
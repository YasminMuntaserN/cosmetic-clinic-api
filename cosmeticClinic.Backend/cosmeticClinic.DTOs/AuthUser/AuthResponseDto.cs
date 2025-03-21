namespace cosmeticClinic.DTOs.Doctor;

public class AuthResponseDto
{
    public string RefreshToken { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public UserDto UserDTO { get; set; } = null!;
}
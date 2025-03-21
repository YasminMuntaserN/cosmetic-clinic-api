namespace cosmeticClinic.DTOs.Doctor;

public class DoctorDto
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Specialization { get; set; } = null!;
    public string? ImageUrl { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public bool IsAvailable { get; set; }
    public WorkingHoursDto[] WorkingHours { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
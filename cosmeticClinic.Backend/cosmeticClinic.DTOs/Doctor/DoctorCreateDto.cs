namespace cosmeticClinic.DTOs.Doctor;

public class DoctorCreateDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Specialization { get; set; } = null!;
    public string? ImageUrl { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public WorkingHoursDto[] WorkingHours { get; set; } = null!;
}
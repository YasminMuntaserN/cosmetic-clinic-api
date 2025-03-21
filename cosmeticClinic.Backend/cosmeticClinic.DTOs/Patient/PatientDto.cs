namespace cosmeticClinic.DTOs;

public class PatientDto
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public AddressDto Address { get; set; } = null!;
    public List<MedicalHistoryDto> MedicalHistory { get; set; } = new();
    public List<string> Allergies { get; set; } = new();
    public string EmergencyContact { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}







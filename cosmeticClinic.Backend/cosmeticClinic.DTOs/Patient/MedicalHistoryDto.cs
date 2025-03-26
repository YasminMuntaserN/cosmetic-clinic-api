namespace cosmeticClinic.DTOs;

public class MedicalHistoryDto
{
    public string Condition { get; set; } = null!;
    public string Diagnosis { get; set; } = null!;
    public DateTime DiagnosisDate { get; set; }
    public List<string> Medications { get; set; } = new();
}

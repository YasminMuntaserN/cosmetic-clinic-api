namespace cosmeticClinic.DTOs.Appointment;

public class AppointmentCreateDto
{
    public string PatientId { get; set; } = null!;
    public string DoctorId { get; set; } = null!;
    public string TreatmentId { get; set; } = null!;
    public DateTime ScheduledDateTime { get; set; }
    public int DurationMinutes { get; set; }
    public string? Notes { get; set; }
}
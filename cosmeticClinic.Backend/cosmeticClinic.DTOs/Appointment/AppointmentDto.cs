using cosmeticClinic.Entities;

namespace cosmeticClinic.DTOs.Appointment;

public class AppointmentDto
{
    public string Id { get; set; } = null!;
    public string PatientId { get; set; } = null!;
    public string DoctorId { get; set; } = null!;
    public string TreatmentId { get; set; } = null!;
    public DateTime ScheduledDateTime { get; set; }
    public int DurationMinutes { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


using cosmeticClinic.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cosmeticClinic.DTOs.Appointment;

public class AppointmentDetailsDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public string PatientName { get; set; } = null!;
    public string DoctorName { get; set; } = null!;
    public string TreatmentName { get; set; } = null!;
    [BsonElement("scheduledDateTime")]
    public DateTime? ScheduledDateTime { get; set; }
    public int DurationMinutes { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


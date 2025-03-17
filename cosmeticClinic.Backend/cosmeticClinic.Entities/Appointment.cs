using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cosmeticClinic.Entities;

public class Appointment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("patientId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string PatientId { get; set; } = null!;

    [BsonElement("doctorId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string DoctorId { get; set; } = null!;

    [BsonElement("treatmentId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string TreatmentId { get; set; } = null!;

    [BsonElement("scheduledDateTime")]
    public DateTime ScheduledDateTime { get; set; }

    [BsonElement("duration")]
    public int DurationMinutes { get; set; }

    [BsonElement("status")]
    public AppointmentStatus Status { get; set; }

    [BsonElement("notes")]
    public string? Notes { get; set; }

    [BsonElement("cancellationReason")]
    public string? CancellationReason { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("isDeleted")]
    public bool IsDeleted { get; set; } = false;
}

public enum AppointmentStatus
{
    Scheduled,
    Confirmed,
    InProgress,
    Completed,
    Cancelled,
    NoShow
}
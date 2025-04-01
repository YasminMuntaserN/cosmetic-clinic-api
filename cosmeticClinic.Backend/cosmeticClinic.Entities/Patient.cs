using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cosmeticClinic.Entities;

public class Patient
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("userId")] 
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = null!;

    
    [BsonElement("firstName")]
    public string FirstName { get; set; } = null!;

    [BsonElement("lastName")]
    public string LastName { get; set; } = null!;

    [BsonElement("email")]
    public string Email { get; set; } = null!;

    [BsonElement("phone")]
    public string Phone { get; set; } = null!;

    [BsonElement("dateOfBirth")]
    public DateTime DateOfBirth { get; set; }

    [BsonElement("gender")]
    public string Gender { get; set; } = null!;

    [BsonElement("address")]
    public Address Address { get; set; } = null!;

    [BsonElement("medicalHistory")]
    public List<MedicalHistory> MedicalHistory { get; set; } = new();

    [BsonElement("allergies")]
    public List<string> Allergies { get; set; } = new();

    [BsonElement("emergencyContact")]
    public string EmergencyContact { get; set; } = null!;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("isDeleted")]
    public bool IsDeleted { get; set; } = false;
}

public class Address
{
    [BsonElement("street")]
    public string Street { get; set; } = null!;

    [BsonElement("city")]
    public string City { get; set; } = null!;

    [BsonElement("state")]
    public string State { get; set; } = null!;

    [BsonElement("postalCode")]
    public string PostalCode { get; set; } = null!;

    [BsonElement("country")]
    public string Country { get; set; } = null!;
}

public class MedicalHistory
{
    [BsonElement("condition")]
    public string Condition { get; set; } = null!;

    [BsonElement("diagnosis")]
    public string Diagnosis { get; set; } = null!;

    [BsonElement("diagnosisDate")]
    public DateTime DiagnosisDate { get; set; }

    [BsonElement("medications")]
    public List<string> Medications { get; set; } = new();
}


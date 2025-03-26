using System.Linq.Expressions;
using AutoMapper;
using cosmeticClinic.Business.Base;
using cosmeticClinic.DTOs.Appointment;
using cosmeticClinic.DTOs.Common;
using cosmeticClinic.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace cosmeticClinic.Business;

public class AppointmentService : BaseService<Appointment, AppointmentDto>
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<AppointmentService> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<Appointment> _validator;
    private readonly IMongoCollection<Appointment> _collection;
    public AppointmentService(
        IMongoDatabase database,
        ILogger<AppointmentService> logger,
        IMapper mapper,
        IValidator<Appointment> validator)
        : base(database, "appointments", logger, mapper, validator)
    {
        _collection =database.GetCollection<Appointment>("appointments"); 
        _logger = logger;
        _mapper = mapper;
        _validator = validator;
        _database = database;
    }

    public async Task<AppointmentDto> AddAppointmentAsync(AppointmentCreateDto appointmentCreateDto)
        => await AddAsync(appointmentCreateDto, "Appointment");

    public async Task<AppointmentDto?> UpdateAppointmentAsync(string id, AppointmentDto appointmentDto)
        => await UpdateAsync(id, appointmentDto, "Appointment");

    public async Task<AppointmentDto?> GetAppointmentByIdAsync(string id)
        => await FindBy(a => a.Id == id);

public async Task<IEnumerable<AppointmentDetailsDto>> GetAllAppointmentsAsync()
{
    // Retrieve all appointments
    var appointments = await _collection
        .Find(a => !a.IsDeleted)
        .ToListAsync();

    // Get patient, doctor, and treatment information by ID
    var patientIds = appointments.Select(a => a.PatientId).Distinct().ToList();
    var doctorIds = appointments.Select(a => a.DoctorId).Distinct().ToList();
    var treatmentIds = appointments.Select(a => a.TreatmentId).Distinct().ToList();

    var patients = await _database.GetCollection<Patient>("patients")
        .Find(p => patientIds.Contains(p.Id))
        .ToListAsync();

    var doctors = await _database.GetCollection<Doctor>("doctors")
        .Find(d => doctorIds.Contains(d.Id))
        .ToListAsync();

    var treatments = await _database.GetCollection<Treatment>("treatments")
        .Find(t => treatmentIds.Contains(t.Id))
        .ToListAsync();

    // Create a dictionary for easy lookup by ID
    var patientDictionary = patients.ToDictionary(p => p.Id.ToString(), p => p);
    var doctorDictionary = doctors.ToDictionary(d => d.Id.ToString(), d => d);
    var treatmentDictionary = treatments.ToDictionary(t => t.Id.ToString(), t => t);

    // Combine appointment data with related patient, doctor, and treatment information
    var appointmentDtos = appointments.Select(a =>
    {
        var patient = patientDictionary.GetValueOrDefault(a.PatientId.ToString());
        var doctor = doctorDictionary.GetValueOrDefault(a.DoctorId.ToString());
        var treatment = treatmentDictionary.GetValueOrDefault(a.TreatmentId.ToString());

        return new AppointmentDetailsDto
        {
            Id = a.Id.ToString(),
            PatientName = $"{patient?.FirstName} {patient?.LastName}",
            DoctorName = $"{doctor?.FirstName} {doctor?.LastName}",
            TreatmentName = treatment?.Name,
            ScheduledDateTime = a.ScheduledDateTime,
            DurationMinutes = a.DurationMinutes,
            Status = a.Status,
            Notes = a.Notes,
            CancellationReason = a.CancellationReason,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        };
    }).ToList();

    return appointmentDtos;
}



    public async Task<PaginatedResponseDto<AppointmentDto>> GetAllAppointmentsAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        bool ascending = true)
    {
        Expression<Func<Appointment, object>> orderByExpression = orderBy.ToLower() switch
        {
            "scheduledDateTime" => a => a.ScheduledDateTime,
            "status" => a => a.Status,
            "createdAt" => a => a.CreatedAt,
            _ => a => a.ScheduledDateTime
        };

        return await GetAllAsync(pageNumber, pageSize, orderByExpression, ascending);
    }

    public async Task<bool> HardDeleteAppointmentAsync(string id)
        => await HardDeleteAsync(id);

    public async Task<bool> SoftDeleteAppointmentAsync(string id)
        => await SoftDeleteAsync(id, "IsDeleted");

    public async Task<bool> ExistsAppointmentAsync(string id)
        => await ExistsAsync(id);
    

    public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        => await SearchAsync(a => a.ScheduledDateTime >= startDate && a.ScheduledDateTime <= endDate);

    public async Task<IEnumerable<AppointmentDto>> SearchAppointmentsAsync(SearchCriteriaDto criteria)
    {
        var regexFilter = criteria.Field switch
        {
            "patientId" => Builders<Appointment>.Filter.Regex(c => c.PatientId,
                new BsonRegularExpression(criteria.Value, "i")),
            "doctorId" => Builders<Appointment>.Filter.Regex(c => c.DoctorId,
                new BsonRegularExpression(criteria.Value, "i")),
            _ => Builders<Appointment>.Filter.Eq("_id", ObjectId.Parse(criteria.Value))
        };

        return await SearchAsync(c => regexFilter.Inject());
    }
}
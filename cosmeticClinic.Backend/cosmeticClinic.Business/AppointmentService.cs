using System.Linq.Expressions;
using AutoMapper;
using cosmeticClinic.Business.Base;
using cosmeticClinic.DTOs.Appointment;
using cosmeticClinic.DTOs.Common;
using cosmeticClinic.DTOs.Doctor;
using cosmeticClinic.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace cosmeticClinic.Business;

public class AppointmentService : BaseService<Appointment, AppointmentDto>
{
    private readonly ILogger<AppointmentService> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<Appointment> _validator;

    public AppointmentService(
        IMongoDatabase database,
        ILogger<AppointmentService> logger,
        IMapper mapper,
        IValidator<Appointment> validator)
        : base(database, "appointments", logger, mapper, validator)
    {
        _logger = logger;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<AppointmentDto> AddAppointmentAsync(AppointmentCreateDto appointmentCreateDto)
        => await AddAsync(appointmentCreateDto, "Appointment");

    public async Task<AppointmentDto?> UpdateAppointmentAsync(string id, AppointmentDto appointmentDto)
        => await UpdateAsync(id, appointmentDto, "Appointment");

    public async Task<AppointmentDto?> GetAppointmentByIdAsync(string id)
        => await FindBy(a => a.Id == id);

    public async Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync()
        => await GetAllAsync();

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
        var filter = criteria.Field switch
        {
            "patientId" => Builders<Appointment>.Filter.Eq(c => c.PatientId,criteria.Value),
            "doctorId" => Builders<Appointment>.Filter.Eq(c => c.DoctorId,criteria.Value),
            _ =>  Builders<Appointment>.Filter.Eq("_id", ObjectId.Parse(criteria.Value)),
        };

        if (filter != null)
        {
            return await SearchAsync(c => filter.Inject());
        }

        return Enumerable.Empty<AppointmentDto>();
    }
}
using System.Linq.Expressions;
using AutoMapper;
using cosmeticClinic.Business.Base;
using cosmeticClinic.DTOs;
using cosmeticClinic.DTOs.Common;
using cosmeticClinic.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace cosmeticClinic.Business;

public class PatientService : BaseService<Patient, PatientDto>
{
    private readonly ILogger<PatientService> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<Patient> _validator;

    public PatientService(
        IMongoDatabase database,
        ILogger<PatientService> logger,
        IMapper mapper,
        IValidator<Patient> validator)
        : base(database, "patients", logger, mapper, validator)
    {
        _logger = logger;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<PatientDto> AddPatientAsync(PatientCreateDto patientCreateDto)
        => await AddAsync(patientCreateDto, "Patient");

    public async Task<PatientDto?> UpdatePatientAsync(string id, PatientDto patientDto)
        => await UpdateAsync(id, patientDto, "Patient");

    public async Task<PatientDto?> GetPatientByIdAsync(string id)
        => await FindBy(p => p.Id == id);

    public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        => await GetAllAsync();

    public async Task<(IEnumerable<PatientDto> Data, long TotalCount, int TotalPages)> GetAllPatientsAsync(
        int pageNumber,
        int pageSize,
        string orderBy = "LastName",
        bool ascending = true)
    {
        Expression<Func<Patient, object>> orderByExpression = orderBy switch
        {
            "firstName" => p => p.FirstName,
            "lastName" => p => p.LastName,
            "email" => p => p.Email,
            _ => p => p.Id
        };

        return await GetAllAsync(pageNumber, pageSize, orderByExpression, ascending);
    }

    public async Task<bool> HardDeletePatientAsync(string id)
        => await HardDeleteAsync(id);

    public async Task<bool> SoftDeletePatientAsync(string id)
        => await SoftDeleteAsync(id, "IsDeleted");

    public async Task<bool> ExistsPatientAsync(string id)
        => await ExistsAsync(id);
    
    
    public async Task<IEnumerable<PatientDto>> SearchPatientsAsync(SearchCriteriaDto criteria)
    {
        var filter = criteria.Field switch
        {
            "firstName" => Builders<Patient>.Filter.Eq(c => c.FirstName,criteria.Value),
            "lastName" => Builders<Patient>.Filter.Eq(c => c.LastName,criteria.Value),
            "email" => Builders<Patient>.Filter.Eq(c => c.Email,criteria.Value),
            _ =>  Builders<Patient>.Filter.Eq("_id", ObjectId.Parse(criteria.Value)),
        };

        if (filter != null)
        {
            return await SearchAsync(c => filter.Inject());
        }

        return Enumerable.Empty<PatientDto>();
    }
}
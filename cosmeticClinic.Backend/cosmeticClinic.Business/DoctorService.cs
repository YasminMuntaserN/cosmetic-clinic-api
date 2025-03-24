using System.Linq.Expressions;
using AutoMapper;
using cosmeticClinic.Business.Base;
using cosmeticClinic.DTOs.Common;
using cosmeticClinic.DTOs.Doctor;
using cosmeticClinic.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace cosmeticClinic.Business;

public class DoctorService : BaseService<Doctor, DoctorDto>
{
    private readonly ILogger<DoctorService> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<Doctor> _validator;

    public DoctorService(
        IMongoDatabase database,
        ILogger<DoctorService> logger,
        IMapper mapper,
        IValidator<Doctor> validator)
        : base(database, "doctors", logger, mapper, validator)
    {
        _logger = logger;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<DoctorDto> AddDoctorAsync(DoctorCreateDto doctorCreateDto)
        => await AddAsync(doctorCreateDto, "Doctor");

    public async Task<DoctorDto?> UpdateDoctorAsync(string id, DoctorDto doctorDto)
        => await UpdateAsync(id, doctorDto, "Doctor");

    public async Task<DoctorDto?> GetDoctorByIdAsync(string id)
        => await FindBy(d => d.Id == id);

    public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        => await GetAllAsync();

    public async Task<PaginatedResponseDto<DoctorDto>> GetAllDoctorsAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        bool ascending = true)
    {

        Expression<Func<Doctor, object>> orderByExpression = orderBy switch
        {
            "firstName" => d => d.FirstName,
            "lastName" => d => d.LastName,
            "specialization" => d => d.Specialization,
            "createdAt" => d => d.CreatedAt,
            _ => d => d.Id
        };

        return await GetAllAsync(pageNumber, pageSize, orderByExpression, ascending);
    }

    public async Task<bool> HardDeleteDoctorAsync(string id)
        => await HardDeleteAsync(id);

    public async Task<bool> SoftDeleteDoctorAsync(string id)
        => await SoftDeleteAsync(id, "IsDeleted");

    public async Task<bool> ExistsDoctorAsync(string id)
        => await ExistsAsync(id);

    public async Task<IEnumerable<DoctorDto>> SearchDoctorsAsync(SearchCriteriaDto criteria)
    {

        var regexFilter = criteria.Field switch
        {
            "firstName" => Builders<Doctor>.Filter.Regex(c => c.FirstName,
                new BsonRegularExpression(criteria.Value, "i")),
            "lastName" => Builders<Doctor>.Filter.Regex(c => c.LastName,
                new BsonRegularExpression(criteria.Value, "i")),
            "specialization" => Builders<Doctor>.Filter.Regex(c => c.Specialization,
                new BsonRegularExpression(criteria.Value, "i")),
            _ => Builders<Doctor>.Filter.Eq("_id", ObjectId.Parse(criteria.Value))
        };

        return await SearchAsync(c => regexFilter.Inject());
    }
}

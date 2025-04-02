using System.Linq.Expressions;
using AutoMapper;
using cosmeticClinic.Business.Base;
using cosmeticClinic.DTOs;
using cosmeticClinic.DTOs.Common;
using cosmeticClinic.Entities;
using cosmeticClinic.Settings;
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
    private readonly IMongoCollection<Patient> _collection;
    private readonly IMongoCollection<User> _userCollection;
    private readonly PasswordSettings _passwordSettings;
    private readonly EmailService _emailService;
    
    public PatientService(
        IMongoDatabase database,
        ILogger<PatientService> logger,
        IMapper mapper,
        PasswordSettings passwordSettings ,
        EmailService emailService ,
        IValidator<Patient> validator)
        : base(database, "patients", logger, mapper, validator)
    {
        _collection =database.GetCollection<Patient>("patients"); 
        _userCollection = database.GetCollection<User>("users");
        _logger = logger;
        _mapper = mapper;
        _validator = validator;
        _passwordSettings = passwordSettings;
        _emailService = emailService;
    }

    public async Task<PatientDto> AddPatientAsync(PatientCreateDto patientCreateDto)
    {
        try
        {
            var entity = _mapper.Map<Patient>(patientCreateDto);

            var validationResult = await _validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed while creating new patient: {Errors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new ValidationException(validationResult.Errors.ToString());
            }

            //default password
            var password = "Password";
            
            var user = new User
            {
                Email = patientCreateDto.Email,
                PasswordHash = _passwordSettings.HashPassword(password),
                FirstName = patientCreateDto.FirstName,
                LastName = patientCreateDto.LastName,
                Role = "Patient",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Status = UserStatus.Offline
            };

            await _userCollection.InsertOneAsync(user);
            
            entity.UserId = user.Id;
            
            await _collection.InsertOneAsync(entity);
            
            await _emailService.SendAccountCreatedEmail(entity.UserId  ,entity.Email);
            
            return _mapper.Map<PatientDto>(entity);
        }
        catch (Exception ex)
        {        Console.WriteLine($"Error creating new  +{ex.Message}");
            _logger.LogError(ex, "Error creating new Patient");
            throw;
        }
    }

    public async Task<PatientDto?> UpdatePatientAsync(string id, PatientDto patientDto)
        => await UpdateAsync(id, patientDto, "Patient");

    public async Task<PatientDto?> GetPatientByIdAsync(string id)
        => await FindBy(p => p.Id == id);

    public async Task<IEnumerable<string>> GetAllPatientsNamesAsync()
    {
        var doctorNames = await _collection
            .Find(_ => true) 
            .Project(d => d.FirstName+ " " + d.LastName) 
            .ToListAsync();

        return doctorNames;
    }
    
    public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        => await GetAllAsync();

    public async Task<PaginatedResponseDto<PatientDto>> GetAllPatientsAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
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
        var regexFilter = criteria.Field switch
        {
            "firstName" => Builders<Doctor>.Filter.Regex(c => c.FirstName,
                new BsonRegularExpression(criteria.Value, "i")),
            "lastName" => Builders<Doctor>.Filter.Regex(c => c.LastName,
                new BsonRegularExpression(criteria.Value, "i")),
            "email" => Builders<Doctor>.Filter.Regex(c => c.Email,
                new BsonRegularExpression(criteria.Value, "i")),
            _ => Builders<Doctor>.Filter.Eq("_id", ObjectId.Parse(criteria.Value))
        };

        return await SearchAsync(c => regexFilter.Inject());
    }
}
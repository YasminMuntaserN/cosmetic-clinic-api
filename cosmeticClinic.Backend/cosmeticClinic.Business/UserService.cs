using System.Linq.Expressions;
using AutoMapper;
using cosmeticClinic.Business.Base;
using cosmeticClinic.DTOs.Common;
using cosmeticClinic.DTOs.Doctor;
using cosmeticClinic.Entities;
using cosmeticClinic.Settings;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace cosmeticClinic.Business;

public class UserService : BaseService<User, UserDto>
{
    private readonly ILogger<UserService> _logger;
    private readonly IMongoDatabase _database;
    private readonly IMapper _mapper;
    private readonly IValidator<User> _validator;
    private readonly PasswordSettings _passwordService;

    public UserService(
        IMongoDatabase database,
        ILogger<UserService> logger,
        IMapper mapper,
        PasswordSettings passwordService,
        IValidator<User> validator)
        : base(database, "users", logger, mapper, validator)
    {
        _passwordService =passwordService;
        _logger = logger;
        _mapper = mapper;
        _validator = validator;
        _database =database;
    }

    public async Task<UserDto> AddUserAsync(UserCreateDto createDto)
    {
        var user = _mapper.Map<User>(createDto);
        
        if (createDto is UserCreateDto userCreateDto)
        {
            user.PasswordHash = _passwordService.HashPassword(userCreateDto.PasswordHash);
        }

        return await AddAsync(user, "User");
    }

    public async Task<UserDto?> UpdateUserAsync(string id, UserDto userDetailsDto)
    {
        var user = _mapper.Map<User>(userDetailsDto);
        
        if (userDetailsDto is UserDto userDTO)
        {
            user.PasswordHash = _passwordService.HashPassword(userDTO.PasswordHash);
        }
       
        return await UpdateAsync(id, userDetailsDto, "User");
    }

    public async Task<bool> UpdatePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await FindBy(u => u.Id == userId);

        if (user == null || !_passwordService.VerifyPassword(currentPassword, user.PasswordHash))
            return false;

        var newPasswordHash = _passwordService.HashPassword(newPassword);

        var updateDefinition = Builders<User>.Update
            .Set(u => u.PasswordHash, newPasswordHash);

        await  _database.GetCollection<User>("users").UpdateOneAsync(u => u.Id == userId, updateDefinition);
        return true;
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
        => await FindBy(d => d.Id == id);

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        => await GetAllAsync();

    public async Task<PaginatedResponseDto<UserDto>> GetAllUsersAsync(
        int pageNumber,
        int pageSize,
        string orderBy ,
        bool ascending = true)
    {
        
        Expression<Func<User, object>> orderByExpression = orderBy switch
        {
            "firstName" => d => d.FirstName,
            "lastName" => d => d.LastName,
            "email" => d => d.Email,
            "createdAt" => d => d.CreatedAt,
            _ => d => d.Id
        };

        return await GetAllAsync(pageNumber, pageSize, orderByExpression, ascending);
    }

    public async Task<bool> HardDeleteUserAsync(string id)
        => await HardDeleteAsync(id);

    public async Task<bool> SoftDeleteUserAsync(string id)
        => await SoftDeleteAsync(id, "IsDeleted");

    public async Task<bool> ExistsUserAsync(string id)
        => await ExistsAsync(id);

    public async Task<IEnumerable<UserDto>> SearchUsersAsync(SearchCriteriaDto criteria)
    {
        var filter = criteria.Field switch
        {
            "firstName" => Builders<User>.Filter.Eq(c => c.FirstName,criteria.Value),
            "lastName" => Builders<User>.Filter.Eq(c => c.LastName,criteria.Value),
            "email" => Builders<User>.Filter.Eq(c => c.Email,criteria.Value),
            _ =>  Builders<User>.Filter.Eq("_id", ObjectId.Parse(criteria.Value)),
        };

        if (filter != null)
        {
            return await SearchAsync(c => filter.Inject());
        }

        return Enumerable.Empty<UserDto>();
    }
}
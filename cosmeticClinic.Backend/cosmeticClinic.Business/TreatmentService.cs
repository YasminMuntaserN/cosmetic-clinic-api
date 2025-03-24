using System.Linq.Expressions;
using AutoMapper;
using cosmeticClinic.Business.Base;
using cosmeticClinic.DTOs.Common;
using cosmeticClinic.DTOs.Product;
using cosmeticClinic.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace cosmeticClinic.Business;

public class TreatmentService : BaseService<Treatment, TreatmentDto>
{
    private readonly ILogger<TreatmentService> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<Treatment> _validator;

    public TreatmentService(
        IMongoDatabase database,
        ILogger<TreatmentService> logger,
        IMapper mapper,
        IValidator<Treatment> validator)
        : base(database, "treatments", logger, mapper, validator)
    {
        _logger = logger;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<TreatmentDto> AddTreatmentAsync(TreatmentCreateDto TreatmentCreateDto)
        => await AddAsync(TreatmentCreateDto, "Treatment");

    public async Task<TreatmentDto?> UpdateTreatmentAsync(string id, TreatmentDto TreatmentDto)
        => await UpdateAsync(id, TreatmentDto, "Treatment");

    public async Task<TreatmentDto?> GetTreatmentByIdAsync(string id)
        => await FindBy(p => p.Id == id);

    public async Task<IEnumerable<TreatmentDto>> GetAllTreatmentsAsync()
        => await GetAllAsync();

    public async Task<PaginatedResponseDto<TreatmentDto>> GetAllTreatmentsAsync(
        int pageNumber,
        int pageSize,
        string orderBy ,
        bool ascending = true)
    {
        Expression<Func<Treatment, object>> orderByExpression = orderBy switch
        {
            "name" => p => p.Name,
            "category" => p => p.Category,
            "price" => p => p.Price,
            "createdAt" => p => p.CreatedAt,
            _ => p => p.Id
        };

        return await GetAllAsync(pageNumber, pageSize, orderByExpression, ascending);
    }

    public async Task<bool> HardDeleteTreatmentAsync(string id)
        => await HardDeleteAsync(id);

    public async Task<bool> SoftDeleteTreatmentAsync(string id)
        => await SoftDeleteAsync(id, "IsDeleted");

    public async Task<bool> ExistsTreatmentAsync(string id)
        => await ExistsAsync(id);

    public async Task<IEnumerable<TreatmentDto>> SearchTreatmentsAsync(SearchCriteriaDto criteria)
    {
        var regexFilter = criteria.Field switch
        {       
            "name" => Builders<Treatment>.Filter.Regex(c => c.Name,
                new BsonRegularExpression(criteria.Value, "i")),
            _ => Builders<Treatment>.Filter.Eq("_id", ObjectId.Parse(criteria.Value))
        };

        return await SearchAsync(c => regexFilter.Inject());
    }

    public async Task<IEnumerable<TreatmentDto>> GetTreatmentsByCategoryAsync(string category)
    {
        if (!Enum.TryParse<TreatmentCategory>(category, true, out var parsedCategory))
        {
            _logger.LogWarning("Invalid category provided: {Category}", category);
            return Enumerable.Empty<TreatmentDto>();
        }

        return await SearchAsync(p => p.Category == parsedCategory);
    }

    public async Task<IEnumerable<TreatmentDto>> GetTreatmentsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        => await SearchAsync(p => p.Price >= minPrice && p.Price <= maxPrice);
}
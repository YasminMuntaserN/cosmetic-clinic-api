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

public class ProductService : BaseService<Product, ProductDto>
{
    private readonly ILogger<ProductService> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<Product> _validator;

    public ProductService(
        IMongoDatabase database,
        ILogger<ProductService> logger,
        IMapper mapper,
        IValidator<Product> validator)
        : base(database, "products", logger, mapper, validator)
    {
        _logger = logger;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<ProductDto> AddProductAsync(ProductCreateDto productCreateDto)
        => await AddAsync(productCreateDto, "Product");

    public async Task<ProductDto?> UpdateProductAsync(string id, ProductDto productDto)
        => await UpdateAsync(id, productDto, "Product");

    public async Task<ProductDto?> GetProductByIdAsync(string id)
        => await FindBy(p => p.Id == id);

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        => await GetAllAsync();

    public async Task<PaginatedResponseDto<ProductDto>> GetAllProductsAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        bool ascending = true)
    {
        Expression<Func<Product, object>> orderByExpression = orderBy switch
        {
            "name" => p => p.Name,
            "category" => p => p.Category,
            "price" => p => p.Price,
            "stockQuantity" => p => p.StockQuantity,
            "createdAt" => p => p.CreatedAt,
            _ => p => p.Id
        };

        return await GetAllAsync(pageNumber, pageSize, orderByExpression, ascending);
    }

    public async Task<bool> HardDeleteProductAsync(string id)
        => await HardDeleteAsync(id);

    public async Task<bool> SoftDeleteProductAsync(string id)
        => await SoftDeleteAsync(id, "IsDeleted");

    public async Task<bool> ExistsProductAsync(string id)
        => await ExistsAsync(id);


    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category)
    {
        if (!Enum.TryParse<ProductCategory>(category, true, out var parsedCategory))
        {
            _logger.LogWarning("Invalid category provided: {Category}", category);
            return Enumerable.Empty<ProductDto>();
        }

        return await SearchAsync(p => p.Category == parsedCategory);
    }


    public async Task<IEnumerable<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        => await SearchAsync(p => p.Price >= minPrice && p.Price <= maxPrice);


    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(SearchCriteriaDto criteria)
    {
        var regexFilter = criteria.Field switch
        {
            "name" => Builders<Product>.Filter.Regex(p => p.Name, new BsonRegularExpression(criteria.Value, "i")),
            "category" => Builders<Product>.Filter.Regex(p => p.Category,
                new BsonRegularExpression(criteria.Value, "i")),
            _ => Builders<Product>.Filter.Eq("_id", ObjectId.Parse(criteria.Value)),
        };

        return await SearchAsync(p => regexFilter.Inject());
    }
}
using System.Linq.Expressions;
using AutoMapper;
using cosmeticClinic.Business.Interfaces;
using cosmeticClinic.DTOs.Common;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace cosmeticClinic.Business.Base;

public abstract class BaseService<TEntity, TDto> : IBaseService<TEntity, TDto> 
    where TEntity : class 
    where TDto : class
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<TEntity> _collection;
    private readonly ILogger<BaseService<TEntity, TDto>> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<TEntity> _validator;

    protected BaseService(
        IMongoDatabase database,
        string collectionName,
        ILogger<BaseService<TEntity, TDto>> logger,
        IMapper mapper,
        IValidator<TEntity> validator)
    {
        _database = database;
        _collection = database.GetCollection<TEntity>(collectionName);
        _logger = logger;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<TDto?> FindBy(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var entity = await _collection.Find(predicate).FirstOrDefaultAsync();
            return entity != null ? _mapper.Map<TDto>(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving entity");
            throw;
        }
    }

    public async Task<IEnumerable<TDto>> GetAllAsync()
    {
        try
        {
            var entities = await _collection.Find(_ => true).ToListAsync();
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all entities");
            throw;
        }
    }

    public async Task<PaginatedResponseDto<TDto>> GetAllAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, object>> orderBy,
        bool ascending = true)
    {
        try
        {
            var filter = Builders<TEntity>.Filter.Empty;
            var totalCount = await _collection.CountDocumentsAsync(filter);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var sort = ascending
                ? Builders<TEntity>.Sort.Ascending(orderBy)
                : Builders<TEntity>.Sort.Descending(orderBy);

            var entities = await _collection
                .Find(filter)
                .Sort(sort)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new PaginatedResponseDto<TDto>
            {
                Data = _mapper.Map<IEnumerable<TDto>>(entities),
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paginated and sorted entities");
            throw;
        }
    }

    public async Task<TDto> AddAsync<TCreateDto>(TCreateDto createDto, string entityName)
    {
        try
        {
            var entity = _mapper.Map<TEntity>(createDto);

           var validationResult = await _validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed while creating new {EntityName}: {Errors}",
                    entityName, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new ValidationException(validationResult.Errors.ToString());
            }

            await _collection.InsertOneAsync(entity);
            return _mapper.Map<TDto>(entity);
        }
        catch (Exception ex)
        {        Console.WriteLine($"Error creating new  +{ex.Message}");
            _logger.LogError(ex, "Error creating new {EntityName}", entityName);
            throw;
        }
    }

    public  async Task<TDto?> UpdateAsync(string id, TDto dto, string entityName)
    {
        try
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", ObjectId.Parse(id));
            var entity = await _collection.Find(filter).FirstOrDefaultAsync();

            if (entity == null)
            {
                _logger.LogWarning("{EntityName} not found: {Id}", entityName, id);
                throw new KeyNotFoundException($"{entityName} with ID {id} not found.");
            }

            _mapper.Map(dto, entity);

            var validationResult = await _validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed during update of {EntityName}: {Errors}",
                    entityName, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new ValidationException(validationResult.Errors.ToString());
            }

            await _collection.ReplaceOneAsync(filter, entity);
            return _mapper.Map<TDto>(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating {EntityName}: {Id}", entityName, id);
            throw;
        }
    }

    public  async Task<bool> SoftDeleteAsync(string id, string propertyName)
    {
        try
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<TEntity>.Update.Set(propertyName, true);

            var result = await _collection.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
            {
                _logger.LogWarning("Entity not found for soft delete: {EntityId}", id);
                return false;
            }

            _logger.LogInformation("Entity soft deleted successfully: {EntityId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while soft-deleting entity: {EntityId}", id);
            throw;
        }
    }
    
    public  async Task<bool> HardDeleteAsync(string id)
    {
        try
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", ObjectId.Parse(id));
            var result = await _collection.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                _logger.LogWarning("Entity not found for hard delete: {id}", id);
                throw new KeyNotFoundException($"Entity with ID {id} not found.");
            }

            _logger.LogInformation("Successfully hard deleted entity with ID {id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hard deleting entity: {id}", id);
            throw;
        }
    }

    public  async Task<bool> HardDeleteByAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var result = await _collection.DeleteManyAsync(predicate);

            if (result.DeletedCount == 0)
            {
                _logger.LogWarning("No entities found for deletion");
                throw new KeyNotFoundException("No entities found for deletion.");
            }

            _logger.LogInformation("Successfully deleted {count} entities", result.DeletedCount);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entities");
            throw;
        }
    }

    public  async Task<bool> ExistsAsync(string id)
    {
        try
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", ObjectId.Parse(id));
            return await _collection.Find(filter).AnyAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of entity with id: {Id}", id);
            throw;
        }
    }

    public  async Task<bool> ExistsByAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            return await _collection.Find(predicate).AnyAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of entity");
            throw;
        }
    }

    public async Task<IEnumerable<TDto>> SearchAsync(Expression<Func<TEntity, bool>> filter)
    {
        try
        {
            var entities = await _collection.Find(filter).ToListAsync();
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products");
            throw;
        }
    }
}
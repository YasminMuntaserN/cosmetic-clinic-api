using System.Linq.Expressions;
using cosmeticClinic.DTOs.Common;

namespace cosmeticClinic.Business.Interfaces;

public interface IBaseService<TEntity, TDto> where TEntity : class where TDto : class
{
    Task<TDto?> FindBy(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TDto>> GetAllAsync();
    Task<PaginatedResponseDto<TDto>> GetAllAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, object>> orderBy,
        bool ascending = true);
    Task<TDto> AddAsync<TCreateDto>(TCreateDto createDto, string entityName);
    Task<TDto?> UpdateAsync(string id, TDto dto, string entityName );
    Task<bool> SoftDeleteAsync(string id , string propertyName);
    Task<bool> HardDeleteAsync(string id);
    Task<bool> HardDeleteByAsync(Expression<Func<TEntity, bool>> predicate);
    Task<bool> ExistsAsync(string id);
    Task<bool> ExistsByAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TDto>> SearchAsync(Expression<Func<TEntity, bool>> filter);
}
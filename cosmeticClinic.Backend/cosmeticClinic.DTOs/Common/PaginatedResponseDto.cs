namespace cosmeticClinic.DTOs.Common;

public class PaginatedResponseDto<T>
{
    public IEnumerable<T> Data { get; set; } = null!;
    public long TotalCount { get; set; }
    public int TotalPages { get; set; }
}
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

public class PaginationDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    [SwaggerSchema(
        "Page number for pagination (starts from 1)",
        Format = "int32",
        Nullable = false
    )]
    public int PageNumber { get; set; } = 1;

    [Required]
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    [SwaggerSchema(
        "Number of items per page (max 100)",
        Format = "int32",
        Nullable = false
    )]
    public int PageSize { get; set; } = 10;

    [SwaggerSchema(
        "Field name to order results by. Available fields depend on the endpoint",
        Format = "string",
        Nullable = true
    )]
    public string OrderBy { get; set; }

    [SwaggerSchema(
        "Sort direction (true for ascending, false for descending)",
        Format = "boolean",
        Nullable = false
    )]
    public bool Ascending { get; set; } = true;


}
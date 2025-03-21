using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace cosmeticClinic.DTOs.Common;

public class SearchCriteriaDto
{
    [Required(ErrorMessage = "Search field is required")]
    [StringLength(50, ErrorMessage = "Field name cannot exceed 50 characters")]
    [SwaggerSchema(
        "Field name to search by. Available fields depend on the endpoint",
        Format = "string",
        Nullable = false
    )]
    public string Field { get; set; }

    [Required(ErrorMessage = "Search value is required")]
    [StringLength(255, ErrorMessage = "Search value cannot exceed 255 characters")]
    [SwaggerSchema(
        "Value to search for",
        Format = "string",
        Nullable = false
    )]
    public string Value { get; set; }
}
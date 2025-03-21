using cosmeticClinic.Backend.Controllers.Base;
using cosmeticClinic.Business;
using cosmeticClinic.DTOs.Common;
using cosmeticClinic.DTOs.Product;
using cosmeticClinic.Settings;
using cosmeticClinic.Settings.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace cosmeticClinic.Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Manage Treatment operations")]
public class TreatmentsController : BaseController
{
    private readonly TreatmentService _TreatmentService;
    private static readonly string[] SearchFields = new[] { "name","category", "price", "createdAt" };

    public TreatmentsController(
        ILogger<TreatmentsController> logger,
        TreatmentService TreatmentService) : base(logger)
    {
        _TreatmentService = TreatmentService;
    }

    [HttpGet]
    [RequirePermission(Permission.ViewTreatments)]
    [SwaggerOperation(Summary = "Get all Treatments")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns all Treatments", typeof(IEnumerable<TreatmentDto>))]
    public async Task<ActionResult<IEnumerable<TreatmentDto>>> GetAll()
    {
        return await HandleResponse(
            () => _TreatmentService.GetAllTreatmentsAsync(),
            "Successfully retrieved all Treatments");
    }
    
    
    [HttpGet("paginated")]
    [RequirePermission(Permission.ViewTreatments)]
    [SwaggerOperation(Summary = "Get paginated Treatments",
    Description = "Retrieves a paginated list of Treatments. Results are ordered according to specified criteria. for example it must be[\"name\", \"category\", \"price\" ,  \"createdAt\" ]  "
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns paginated Treatments")]
    public async Task<ActionResult<PaginatedResponseDto <TreatmentDto>>>
        GetPaginated([FromQuery] [SwaggerParameter("Pagination parameters")]
            PaginationDto pagination)
    {
        if (pagination.PageNumber < 1 || pagination.PageSize < 1)
        {
            return BadRequest(new { message = "Page number and page size must be greater than 0" });
        }

        if (!string.IsNullOrEmpty(pagination.OrderBy) && !SearchFields.Contains(pagination.OrderBy))
        {
            return BadRequest(new { message = $"OrderBy must be one of: {string.Join(", ", SearchFields)}" });
        }
        
        return await HandleResponse(
            () => _TreatmentService.GetAllTreatmentsAsync(pagination.PageNumber, pagination.PageSize, pagination.OrderBy, pagination.Ascending),
            "Successfully retrieved paginated Treatments");
    }

    
    [HttpGet("{id}")]
    [RequirePermission(Permission.ViewTreatments)]
    [SwaggerOperation(Summary = "Get a Treatment by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the requested Treatment", typeof(TreatmentDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Treatment not found")]
    public async Task<ActionResult<TreatmentDto>> GetById(string id)
    {
        return await HandleResponse(
            () => _TreatmentService.GetTreatmentByIdAsync(id),
            $"Successfully retrieved Treatment with ID: {id}");
    }
    
    
    [HttpGet("search")]
    [RequirePermission(Permission.ViewTreatments)]
    [SwaggerOperation(Summary = "Search Treatments  using specified criteria." ,
        Description = " Available search field:name")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns matching Treatments", typeof(IEnumerable<TreatmentDto>))]
    public async Task<ActionResult<IEnumerable<TreatmentDto>>> Search([FromBody] [SwaggerRequestBody("Search criteria")]
        SearchCriteriaDto criteria)
    {
        return await HandleResponse(
            () => _TreatmentService.SearchTreatmentsAsync(criteria),
            "Successfully performed Treatment search");
    }
    
    
    [HttpPost]
    [RequirePermission(Permission.CreateTreatment)]
    [SwaggerOperation(Summary = "Create a new Treatment")]
    [SwaggerResponse(StatusCodes.Status200OK, "Treatment created successfully", typeof(TreatmentDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
    public async Task<ActionResult<TreatmentDto>> Create(TreatmentCreateDto TreatmentCreateDto)
    {
        return await HandleResponse(
            () => _TreatmentService.AddTreatmentAsync(TreatmentCreateDto),
            "Treatment created successfully");
    }

    
    [HttpPut("{id}")]
    [RequirePermission(Permission.MangeTreatment)]
    [SwaggerOperation(Summary = "Update a Treatment")]
    [SwaggerResponse(StatusCodes.Status200OK, "Treatment updated successfully", typeof(TreatmentDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Treatment not found")]
    public async Task<ActionResult<TreatmentDto>> Update(string id, TreatmentDto TreatmentDto)
    {
        return await HandleResponse(
            () => _TreatmentService.UpdateTreatmentAsync(id, TreatmentDto),
            $"Successfully updated Treatment with ID: {id}");
    }

    [HttpDelete("{id}")]
    [RequirePermission(Permission.DeleteTreatment)]
    [SwaggerOperation(Summary = "Delete a Treatment")]
    [SwaggerResponse(StatusCodes.Status200OK, "Treatment deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Treatment not found")]
    public async Task<ActionResult<bool>> Delete(string id)
    {
        return await HandleResponse(
            () => _TreatmentService.SoftDeleteTreatmentAsync(id),
            $"Successfully deleted Treatment with ID: {id}");
    }




}
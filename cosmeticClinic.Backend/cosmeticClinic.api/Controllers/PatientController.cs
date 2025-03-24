using cosmeticClinic.Backend.Controllers.Base;
using cosmeticClinic.Business;
using cosmeticClinic.DTOs;
using cosmeticClinic.DTOs.Common;
using cosmeticClinic.Settings;
using cosmeticClinic.Settings.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace cosmeticClinic.Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Manage Patient operations")]
public class PatientsController : BaseController
{
    private readonly PatientService _PatientService;
    private static readonly string[] SearchFields = new[] { "firstName","lastName", "email" };

    public PatientsController(
        ILogger<PatientsController> logger,
        PatientService PatientService) : base(logger)
    {
        _PatientService = PatientService;
    }

    [HttpGet]
    [RequirePermission(Permission.ViewPatients)]
    [SwaggerOperation(Summary = "Get all Patients")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns all Patients", typeof(IEnumerable<PatientDto>))]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
    {
        return await HandleResponse(
            () => _PatientService.GetAllPatientsAsync(),
            "Successfully retrieved all Patients");
    }
    
    
    [HttpGet("paginated")]
    [RequirePermission(Permission.ViewPatients)]
    [SwaggerOperation(Summary = "Get paginated Patients",
    Description = "Retrieves a paginated list of Patients. Results are ordered according to specified criteria. for example it must be[\"firstName\", \"lastName\", \"email\" ]  "
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns paginated Patients")]
    public async Task<ActionResult<PaginatedResponseDto <PatientDto>>>
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
            () => _PatientService.GetAllPatientsAsync(pagination.PageNumber, pagination.PageSize, pagination.OrderBy, pagination.Ascending),
            "Successfully retrieved paginated Patients");
    }

    
    [HttpGet("{id}")]
    [RequirePermission(Permission.ViewPatients)]
    [SwaggerOperation(Summary = "Get a Patient by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the requested Patient", typeof(PatientDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Patient not found")]
    public async Task<ActionResult<PatientDto>> GetById(string id)
    {
        return await HandleResponse(
            () => _PatientService.GetPatientByIdAsync(id),
            $"Successfully retrieved Patient with ID: {id}");
    }
    
    
    [HttpPost("search")]
    [RequirePermission(Permission.ViewPatients)]
    [SwaggerOperation(Summary = "Search Patients  using specified criteria." , 
        Description = " Available search fields:firstName, lastName, email")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns matching Patients", typeof(IEnumerable<PatientDto>))]
    public async Task<ActionResult<IEnumerable<PatientDto>>> Search([FromBody] [SwaggerRequestBody("Search criteria")]
        SearchCriteriaDto criteria)
    {
        return await HandleResponse(
            () => _PatientService.SearchPatientsAsync(criteria),
            "Successfully performed Patient search");
    }
    
    
    [HttpPost]
    [RequirePermission(Permission.CreatePatient)]
    [SwaggerOperation(Summary = "Create a new Patient")]
    [SwaggerResponse(StatusCodes.Status200OK, "Patient created successfully", typeof(PatientDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
    public async Task<ActionResult<PatientDto>> Create(PatientCreateDto PatientCreateDto)
    {
        return await HandleResponse(
            () => _PatientService.AddPatientAsync(PatientCreateDto),
            "Patient created successfully");
    }

    
    [HttpPut("{id}")]
    [RequirePermission(Permission.MangePatient)]
    [SwaggerOperation(Summary = "Update a Patient")]
    [SwaggerResponse(StatusCodes.Status200OK, "Patient updated successfully", typeof(PatientDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Patient not found")]
    public async Task<ActionResult<PatientDto>> Update(string id, PatientDto PatientDto)
    {
        return await HandleResponse(
            () => _PatientService.UpdatePatientAsync(id, PatientDto),
            $"Successfully updated Patient with ID: {id}");
    }

    [HttpDelete("{id}")]
    [RequirePermission(Permission.DeletePatient)]
    [SwaggerOperation(Summary = "Delete a Patient")]
    [SwaggerResponse(StatusCodes.Status200OK, "Patient deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Patient not found")]
    public async Task<ActionResult<bool>> Delete(string id)
    {
        return await HandleResponse(
            () => _PatientService.SoftDeletePatientAsync(id),
            $"Successfully deleted Patient with ID: {id}");
    }
    
}
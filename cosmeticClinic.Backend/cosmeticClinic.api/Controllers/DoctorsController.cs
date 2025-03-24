using cosmeticClinic.Backend.Controllers.Base;
using cosmeticClinic.Business;
using cosmeticClinic.DTOs.Common;
using cosmeticClinic.DTOs.Doctor;
using cosmeticClinic.Settings;
using cosmeticClinic.Settings.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace cosmeticClinic.Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Manage Doctor operations")]
public class DoctorsController : BaseController
{
    private readonly DoctorService _doctorService;
    private static readonly string[] SearchFields = new[] { "firstName","lastName", "createdAt", "specialization" };

    public DoctorsController(
        ILogger<DoctorsController> logger,
        DoctorService doctorService) : base(logger)
    {
        _doctorService = doctorService;
    }

    [HttpGet]
    [RequirePermission(Permission.ViewDoctors)]
    [SwaggerOperation(Summary = "Get all doctors")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns all doctors", typeof(IEnumerable<DoctorDto>))]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAll()
    {
        return await HandleResponse(
            () => _doctorService.GetAllDoctorsAsync(),
            "Successfully retrieved all doctors");
    }
    
    
    [HttpGet("paginated")]
    [RequirePermission(Permission.ViewDoctors)]
    [SwaggerOperation(Summary = "Get paginated doctors" ,
        Description = "Retrieves a paginated list of doctors. Results are ordered according to specified criteria. for example it must be[\"firstName\", \"lastName\", \"createdAt\", \"specialization\" ]  " )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns paginated doctors")]
    public async Task<ActionResult<PaginatedResponseDto <DoctorDto>>>
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
            () => _doctorService.GetAllDoctorsAsync(pagination.PageNumber, pagination.PageSize, pagination.OrderBy, pagination.Ascending),
            "Successfully retrieved paginated doctors");
    }

    
    [HttpGet("{id}")]
    [RequirePermission(Permission.ViewDoctors)]
    [SwaggerOperation(Summary = "Get a doctor by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the requested doctor", typeof(DoctorDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Doctor not found")]
    public async Task<ActionResult<DoctorDto>> GetById(string id)
    {
        return await HandleResponse(
            () => _doctorService.GetDoctorByIdAsync(id),
            $"Successfully retrieved doctor with ID: {id}");
    }

    
    
    [HttpPost("search")]
    [RequirePermission(Permission.ViewDoctors)]
    [SwaggerOperation(Summary = "Search doctors  using specified criteria." ,
     Description = " Available search fields: FirstName , LastName ,Specialization")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns matching doctors", typeof(IEnumerable<DoctorDto>))]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> Search([FromBody] [SwaggerRequestBody("Search criteria")]
        SearchCriteriaDto criteria)
    {
        return await HandleResponse(
            () => _doctorService.SearchDoctorsAsync(criteria),
            "Successfully performed doctor search");
    }
    
    
    [HttpPost]
    [RequirePermission(Permission.CreateDoctor)]
    [SwaggerOperation(Summary = "Create a new doctor")]
    [SwaggerResponse(StatusCodes.Status200OK, "Doctor created successfully", typeof(DoctorDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
    public async Task<ActionResult<DoctorDto>> Create(DoctorCreateDto doctorCreateDto)
    {
        return await HandleResponse(
            () => _doctorService.AddDoctorAsync(doctorCreateDto),
            "Doctor created successfully");
    }

    
    [HttpPut("{id}")]
    [RequirePermission(Permission.MangeDoctor)]
    [SwaggerOperation(Summary = "Update a doctor")]
    [SwaggerResponse(StatusCodes.Status200OK, "Doctor updated successfully", typeof(DoctorDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Doctor not found")]
    public async Task<ActionResult<DoctorDto>> Update(string id, DoctorDto doctorDto)
    {
        return await HandleResponse(
            () => _doctorService.UpdateDoctorAsync(id, doctorDto),
            $"Successfully updated doctor with ID: {id}");
    }

    [HttpDelete("{id}")]
    [RequirePermission(Permission.DeleteDoctor)]
    [SwaggerOperation(Summary = "Delete a doctor")]
    [SwaggerResponse(StatusCodes.Status200OK, "Doctor deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Doctor not found")]
    public async Task<ActionResult<bool>> Delete(string id)
    {
        return await HandleResponse(
            () => _doctorService.SoftDeleteDoctorAsync(id),
            $"Successfully deleted doctor with ID: {id}");
    }
    
}
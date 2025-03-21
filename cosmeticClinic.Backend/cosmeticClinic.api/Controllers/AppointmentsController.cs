using cosmeticClinic.Backend.Controllers.Base;
using cosmeticClinic.Business;
using cosmeticClinic.DTOs;
using cosmeticClinic.DTOs.Appointment;
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
[SwaggerTag("Manage Appointment operations")]
public class AppointmentsController : BaseController
{
    private readonly AppointmentService _AppointmentService;

    public AppointmentsController(
        ILogger<AppointmentsController> logger,
        AppointmentService AppointmentService) : base(logger)
    {
        _AppointmentService = AppointmentService;
    }

    [HttpGet]
    [RequirePermission(Permission.ViewAppointments)]
    [SwaggerOperation(Summary = "Get all Appointments")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns all Appointments", typeof(IEnumerable<AppointmentDto>))]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAll()
    {
        return await HandleResponse(
            () => _AppointmentService.GetAllAppointmentsAsync(),
            "Successfully retrieved all Appointments");
    }

    [HttpGet("paginated")]
    [RequirePermission(Permission.ViewAppointments)]
    [SwaggerOperation(Summary = "Get paginated Appointments")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns paginated Appointments")]
    public async Task<ActionResult<PaginatedResponseDto <AppointmentDto>>>
        GetPaginated([FromQuery] PaginationDto pagination)
    {
        if (pagination.PageNumber < 1 || pagination.PageSize < 1)
        {
            return BadRequest(new { message = "Page number and page size must be greater than 0" });
        }

        return await HandleResponse(
            () => _AppointmentService.GetAllAppointmentsAsync(pagination.PageNumber, pagination.PageSize, pagination.OrderBy, pagination.Ascending),
            "Successfully retrieved paginated Appointments");
    }

    [HttpGet("{id}")]
    [RequirePermission(Permission.ViewAppointments)]
    [SwaggerOperation(Summary = "Get a Appointment by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the requested Appointment", typeof(AppointmentDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Appointment not found")]
    public async Task<ActionResult<AppointmentDto>> GetById(string id)
    {
        return await HandleResponse(
            () => _AppointmentService.GetAppointmentByIdAsync(id),
            $"Successfully retrieved Appointment with ID: {id}");
    }

    [HttpGet("search")]
    [RequirePermission(Permission.ViewAppointments)]
    [SwaggerOperation(Summary = "Search Appointments using specified criteria.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns matching Appointments", typeof(IEnumerable<AppointmentDto>))]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> Search([FromBody] SearchCriteriaDto criteria)
    {
        return await HandleResponse(
            () => _AppointmentService.SearchAppointmentsAsync(criteria),
            "Successfully performed Appointment search");
    }

    [HttpGet("searchByDateRange")]
    [RequirePermission(Permission.ViewAppointments)]
    [SwaggerOperation(Summary = "Search Appointments By Date Range")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns matching Appointments", typeof(IEnumerable<AppointmentDto>))]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> Search(DateTime startDate, DateTime endDate)
    {
        return await HandleResponse(
            () => _AppointmentService.GetAppointmentsByDateRangeAsync(startDate, endDate),
            "Successfully performed Appointment search");
    }

    [HttpPost]
    [RequirePermission(Permission.CreateAppointment)]
    [SwaggerOperation(Summary = "Create a new Appointment")]
    [SwaggerResponse(StatusCodes.Status200OK, "Appointment created successfully", typeof(AppointmentDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
    public async Task<ActionResult<AppointmentDto>> Create(AppointmentCreateDto appointmentCreateDto)
    {
        return await HandleResponse(
            () => _AppointmentService.AddAppointmentAsync(appointmentCreateDto),
            "Appointment created successfully");
    }

    [HttpPut("{id}")]
    [RequirePermission(Permission.MangeAppointment)]
    [SwaggerOperation(Summary = "Update a Appointment")]
    [SwaggerResponse(StatusCodes.Status200OK, "Appointment updated successfully", typeof(AppointmentDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Appointment not found")]
    public async Task<ActionResult<AppointmentDto>> Update(string id, AppointmentDto appointmentDto)
    {
        return await HandleResponse(
            () => _AppointmentService.UpdateAppointmentAsync(id, appointmentDto),
            $"Successfully updated Appointment with ID: {id}");
    }

    [HttpDelete("{id}")]
    [RequirePermission(Permission.CancelAppointment)]
    [SwaggerOperation(Summary = "Delete a Appointment")]
    [SwaggerResponse(StatusCodes.Status200OK, "Appointment deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Appointment not found")]
    public async Task<ActionResult<bool>> Delete(string id)
    {
        return await HandleResponse(
            () => _AppointmentService.SoftDeleteAppointmentAsync(id),
            $"Successfully deleted Appointment with ID: {id}");
    }
}
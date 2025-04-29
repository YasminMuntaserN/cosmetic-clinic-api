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
[SwaggerTag("Manage User operations")]
public class UsersController : BaseController
{
    private readonly UserService _userService;
    private static readonly string[] SearchFields = new[] { "firstName","LastName", "email" };

    public UsersController(
        ILogger<UsersController> logger,
        UserService userService) : base(logger)
    {
        _userService = userService;
    }

    [HttpGet]
    [RequirePermission(Permission.ManageUsers)]
    [SwaggerOperation(Summary = "Get all Users")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns all Users", typeof(IEnumerable<UserDto>))]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        return await HandleResponse(
            () => _userService.GetAllUsersAsync(),
            "Successfully retrieved all Users");
    }
    
    
    [HttpGet("paginated")]
    [RequirePermission(Permission.ManageUsers)]
    [SwaggerOperation(Summary = "Get paginated Users",
    Description = "Retrieves a paginated list of Users. Results are ordered according to specified criteria. for example it must be[\"firstName\", \"lastName\", \"email\" ]  "
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns paginated Users")]
    public async Task<ActionResult<PaginatedResponseDto <UserDto>>>
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
            () => _userService.GetAllUsersAsync(pagination.PageNumber, pagination.PageSize, pagination?.OrderBy, pagination.Ascending),
            "Successfully retrieved paginated Users");
    }

    
    [HttpGet("{id}")]
    [RequirePermission(Permission.ManageUsers)]
    [SwaggerOperation(Summary = "Get a User by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the requested User", typeof(UserDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
    public async Task<ActionResult<UserDto?>> GetById(string id)
    {
        return await HandleResponse(
            () => _userService.GetUserByIdAsync(id),
            $"Successfully retrieved User with ID: {id}");
    }
    
    
    [HttpPost("search")]
    [RequirePermission(Permission.ManageUsers)]
    [SwaggerOperation(Summary = "Search Users  using specified criteria." ,
        Description = " Available search fields:firstName, lastName, email")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns matching Users", typeof(IEnumerable<UserDto>))]
    public async Task<ActionResult<IEnumerable<UserDto>>> Search([FromBody] [SwaggerRequestBody("Search criteria")]
        SearchCriteriaDto criteria)
    {
        return await HandleResponse(
            () => _userService.SearchUsersAsync(criteria),
            "Successfully performed User search");
    }
    
    
    [HttpPost]
    [RequirePermission(Permission.ManageUsers)]
    [SwaggerOperation(Summary = "Create a new User")]
    [SwaggerResponse(StatusCodes.Status200OK, "User created successfully", typeof(UserDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
    public async Task<ActionResult<UserDto>> Create(UserCreateDto userCreateDto)
    {
        return await HandleResponse(
            () => _userService.AddUserAsync(userCreateDto),
            "User created successfully");
    }

    
    [HttpPut("{id}")]
    [RequirePermission(Permission.ManageUsers)]
    [SwaggerOperation(Summary = "Update a User")]
    [SwaggerResponse(StatusCodes.Status200OK, "User updated successfully", typeof(UserDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
    public async Task<ActionResult<UserDto?>> Update(string id, UserDto userDto)
    {
        return await HandleResponse(
            () => _userService.UpdateUserAsync(id, userDto),
            $"Successfully updated User with ID: {id}");
    }
    
    [HttpPut("")]
    [RequirePermission(Permission.ManageUsers)]
    [SwaggerOperation(Summary = "Change User Password ")]
    [SwaggerResponse(StatusCodes.Status200OK, "User password Changed successfully", typeof(UserDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User  password  not Changed")]
    public async Task<ActionResult<bool>> ChangePassword(string userId, string currentPassword, string newPassword)
    {
        return await HandleResponse(
            () => _userService.UpdatePasswordAsync(userId, currentPassword, newPassword),
            "Successfully updated password for the user");
    }


    [HttpDelete("{id}")]
    [RequirePermission(Permission.ManageUsers)]
    [SwaggerOperation(Summary = "Delete a User")]
    [SwaggerResponse(StatusCodes.Status200OK, "User deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
    public async Task<ActionResult<bool>> Delete(string id)
    {
        return await HandleResponse(
            () => _userService.SoftDeleteUserAsync(id),
            $"Successfully deleted User with ID: {id}");
    }


    [HttpGet("counts")]
    [RequirePermission(Permission.ManageUsers)]
    [SwaggerOperation(Summary = "Get all Data Counts")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns all Stats Counts", typeof(IEnumerable<ReportDto>))]
    public async Task<ActionResult<IEnumerable<ReportDto>>> StatsCount()
    {
        return await HandleResponse(
            () => _userService.StatsCount(),
            "Successfully retrieved all Stats Count");
    }


}
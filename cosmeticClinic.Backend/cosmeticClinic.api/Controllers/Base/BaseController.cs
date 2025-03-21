using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace cosmeticClinic.Backend.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected readonly ILogger<BaseController> _logger;

    protected BaseController(ILogger<BaseController> logger)
    {
        _logger = logger;
    }
    
    protected async Task<ActionResult<T>> HandleResponse<T>(Func<Task<T>> func, string successMessage = "Success")
    {
        try
        {
            var result = await func();

            if (result == null)
            {
                _logger.LogWarning("HandleResponse: No data found.");
                return NotFound(new { message = "No data found." });
            }

            _logger.LogInformation(successMessage);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed: {Message}", ex.Message);

            return BadRequest(new
            {
                message = "Validation failed.",
                error = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An unexpected error occurred."
            });
        }
    }
}
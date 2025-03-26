using cosmeticClinic.Backend.Controllers.Base;
using cosmeticClinic.Business;
using cosmeticClinic.DTOs.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace cosmeticClinic.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly AuthService _authService;

    public AuthController(ILogger<AuthController> logger, AuthService authService)
        : base(logger)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Authenticate user",
        Description = "Authenticates a user and returns access and refresh tokens"
    )]
    [SwaggerResponse(200, "Authentication successful")]
    [SwaggerResponse(401, "Invalid credentials")]
    public async Task<ActionResult<AuthResponseDto>>
        Login([FromBody] AuthRequestDto request)
    {
        return await HandleResponse(
            async () => await _authService.AuthenticateAsync(request),
            "Authentication successful");
    }

    
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Refresh token",
        Description = "Generates new access token using refresh token"
    )]
    [SwaggerResponse(200, "Token refresh successful")]
    [SwaggerResponse(401, "Invalid or expired refresh token")]
    public async Task<ActionResult<(string AccessToken, string RefreshToken)>>
        RefreshToken([FromBody] string refreshToken)
    {
        return await HandleResponse(
            async () => await _authService.RefreshTokenAsync(refreshToken),
            "Token refresh successful");
    }

    
    [HttpPost("revoke")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Revoke refresh token",
        Description = "Revokes the refresh token for the specified user"
    )]
    [SwaggerResponse(200, "Token revoked successfully")]
    public async Task<ActionResult<bool>> RevokeToken([FromBody] string email)
    {
        return await HandleResponse(
            async () =>
            {
                await _authService.RevokeRefreshTokenAsync(email);
                return true;
            },
            "Token revoked successfully");
    }
}
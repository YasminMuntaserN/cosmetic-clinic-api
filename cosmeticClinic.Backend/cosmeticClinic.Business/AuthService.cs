using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using cosmeticClinic.DTOs.Doctor;
using cosmeticClinic.Entities;
using cosmeticClinic.Settings;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace cosmeticClinic.Business;

public class AuthService
{
    private readonly JwtSettings _JwtSettings;
    private readonly IMapper _mapper;
    private readonly IMongoCollection<User> _usersCollection;
    private readonly IMongoCollection<AuthUser> _authUsersCollection;
    private readonly PasswordSettings _PasswordSettings;

    public AuthService(
        JwtSettings JwtSettings,
        IMongoDatabase database,
        IMapper mapper,
        PasswordSettings PasswordSettings)
    {
        _JwtSettings = JwtSettings;
        _usersCollection = database.GetCollection<User>("users");
        _authUsersCollection = database.GetCollection<AuthUser>("authUsers");
        _mapper = mapper;
        _PasswordSettings = PasswordSettings;
    }

    public async Task<AuthResponseDto> AuthenticateAsync(AuthRequestDto request)
    {
        var user = await _usersCollection.Find(x => x.Email == request.Email).FirstOrDefaultAsync();
        if (user == null || !_PasswordSettings.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var authUser = await _authUsersCollection.Find(x => x.UserId == user.Id).FirstOrDefaultAsync();
        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        if (authUser == null)
        {
            authUser = new AuthUser
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_JwtSettings.RefreshTokenLifetimeDays)
            };
            await _authUsersCollection.InsertOneAsync(authUser);
        }
        else
        {
            var updateDefinition = Builders<AuthUser>.Update
                .Set(u => u.RefreshToken, refreshToken)
                .Set(u => u.RefreshTokenExpiryTime, DateTime.UtcNow.AddDays(_JwtSettings.RefreshTokenLifetimeDays));

            await _authUsersCollection.UpdateOneAsync(u => u.AuthUserId == authUser.AuthUserId, updateDefinition);
        }

        var updateUser = Builders<User>.Update.Set(u => u.LastLogin, DateTime.UtcNow);
        await _usersCollection.UpdateOneAsync(u => u.Id == user.Id, updateUser);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserDTO = _mapper.Map<UserDto>(user)
        };
    }

    public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
    {
        var authUser = await _authUsersCollection.Find(u => u.RefreshToken == refreshToken).FirstOrDefaultAsync();
        if (authUser == null || authUser.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        var user = await _usersCollection.Find(u => u.Id == authUser.UserId).FirstOrDefaultAsync();
        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        var updateDefinition = Builders<AuthUser>.Update
            .Set(u => u.RefreshToken, newRefreshToken)
            .Set(u => u.RefreshTokenExpiryTime, DateTime.UtcNow.AddDays(_JwtSettings.RefreshTokenLifetimeDays));

        await _authUsersCollection.UpdateOneAsync(u => u.AuthUserId == authUser.AuthUserId, updateDefinition);

        return (newAccessToken, newRefreshToken);
    }

    private string GenerateJwtToken(User user)
    {
        var permissions = GetPermissionsForRole(user.Role);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role ?? "Patient"),
            new("Permissions", permissions.ToString())
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _JwtSettings.Issuer,
            Audience = _JwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JwtSettings.SignKey)),
                SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_JwtSettings.AccessTokenLifetimeMinutes)
        };

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task RevokeRefreshTokenAsync(string email)
    {
        var user = await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        var updateDefinition = Builders<AuthUser>.Update
            .Set(u => u.RefreshToken, null)
            .Set(u => u.RefreshTokenExpiryTime, null);

        await _authUsersCollection.UpdateOneAsync(u => u.UserId == user.Id, updateDefinition);
    }

    private int GetPermissionsForRole(string role) => role switch
    {
        "Admin" => (int)(
                         Permission.CreateDoctor |
                         Permission.ViewDoctors |
                         Permission.MangeDoctor |
                         Permission.DeleteDoctor |
                         
                         Permission.ViewAppointments |
                         Permission.CreateAppointment |
                         Permission.MangeAppointment |
                         Permission.CancelAppointment |
                         
                         Permission.ViewPatients |
                         Permission.CreatePatient |
                         Permission.MangePatient |
                         Permission.DeletePatient |
                         
                         Permission.ViewTreatments |
                         Permission.CreateTreatment |
                         Permission.MangeTreatment |
                         Permission.DeleteTreatment |
                         
                         Permission.ViewProducts |
                         Permission.CreateProduct |
                         Permission.MangeProduct |
                         Permission.DeleteProduct |
                         
                         Permission.ViewReports |
                         Permission.ManageUsers),

        "Doctor" => (int)(Permission.ViewAppointments |
                          Permission.CreateAppointment |
                          Permission.MangeAppointment |
                          Permission.CancelAppointment |
                          Permission.ViewPatients |
                          Permission.ViewProducts |
                          Permission.ViewTreatments |
                          Permission.ViewDoctors |
                          Permission.CreateTreatment |
                          Permission.MangeTreatment),

        "Patient" => (int)(Permission.ViewDoctors |
                           Permission.ViewProducts |
                           Permission.ViewAppointments |
                           Permission.CreateAppointment |
                           Permission.CancelAppointment),

        _ => 0
    };
}
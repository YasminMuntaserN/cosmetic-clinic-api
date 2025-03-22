using System.Text;
using cosmeticClinic.Business;
using cosmeticClinic.Mappers;
using cosmeticClinic.Settings;
using cosmeticClinic.Settings.Authorization;
using cosmeticClinic.Validation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfig();

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
    return client.GetDatabase(settings.DatabaseName);
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);

//  FluentValidation
// Register FluentValidation
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserValidator>());

// Register Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<TreatmentService>();
builder.Services.AddScoped<PasswordSettings>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Permission.CreateAppointment.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.CreateAppointment)));
    options.AddPolicy(Permission.ViewAppointments.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.ViewAppointments)));
    options.AddPolicy(Permission.MangeAppointment.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.MangeAppointment)));
    options.AddPolicy(Permission.CancelAppointment.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.CancelAppointment)));
    
    options.AddPolicy(Permission.CreateDoctor.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.CreateDoctor)));
    options.AddPolicy(Permission.MangeDoctor.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.MangeDoctor)));
    options.AddPolicy(Permission.DeleteDoctor.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.DeleteDoctor)));
    options.AddPolicy(Permission.ViewDoctors.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.ViewDoctors)));
    
    options.AddPolicy(Permission.CreatePatient.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.CreatePatient)));
    options.AddPolicy(Permission.MangePatient.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.MangePatient)));
    options.AddPolicy(Permission.DeletePatient.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.DeletePatient)));
    options.AddPolicy(Permission.ViewPatients.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.ViewPatients)));
    
    options.AddPolicy(Permission.CreateProduct.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.CreateProduct)));
    options.AddPolicy(Permission.MangeProduct.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.MangeProduct)));
    options.AddPolicy(Permission.DeleteProduct.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.DeleteProduct)));
    options.AddPolicy(Permission.ViewProducts.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.ViewProducts)));
    
    options.AddPolicy(Permission.CreateTreatment.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.CreateTreatment)));
    options.AddPolicy(Permission.MangeTreatment.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.MangeTreatment)));
    options.AddPolicy(Permission.DeleteTreatment.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.DeleteTreatment)));
    options.AddPolicy(Permission.ViewTreatments.ToString(), 
        policy => policy.Requirements.Add(new PermissionRequirement(Permission.ViewTreatments)));
});


var JwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
builder.Services.AddSingleton(JwtSettings);
builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = JwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = JwtSettings.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.SignKey))
    };
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();


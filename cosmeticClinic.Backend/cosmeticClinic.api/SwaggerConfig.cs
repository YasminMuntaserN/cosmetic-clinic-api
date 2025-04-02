using System.Reflection;
using Microsoft.OpenApi.Models;


namespace cosmeticClinic.Settings;

public static class SwaggerConfig
{
    public static void AddSwaggerConfig(this IServiceCollection services)
    {

        services.AddSwaggerGen(c =>
        {
            // Define the Swagger document
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Cosmetic Clinic API",
                Version = "v1",
                Description = "API documentation for the Cosmetic Clinic System",
            });

            // JWT Authentication
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Enable annotations
            c.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);

            // XML Documentation
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
            else
            {
                Console.WriteLine("XML documentation file not found.");
            }
        });
    }
}


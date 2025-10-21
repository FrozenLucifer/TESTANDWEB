using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Detective.Extensions;

public static class SwaggerExtension
{
    public static void AddSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Detective API", Version = "v1" });

            var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);

            foreach (var fileName in xmlFiles)
            {
                var xmlFilePath = Path.Combine(AppContext.BaseDirectory, fileName);
                if (File.Exists(xmlFilePath))
                    options.IncludeXmlComments(xmlFilePath, includeControllerXmlComments: true);
            }
            
            options.EnableAnnotations();

            options.AddSwaggerSecurity();
        });
    }


    private static void AddSwaggerSecurity(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer",
            new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    }
}
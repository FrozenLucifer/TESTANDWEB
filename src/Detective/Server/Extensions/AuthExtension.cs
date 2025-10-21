using System.Text;
using Domain.Configurations;
using Domain.Enum;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Detective.Extensions;

public static class AuthExtension
{
    public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtConfig = configuration.GetRequiredSection(JwtConfiguration.ConfigurationSectionName).Get<JwtConfiguration>();
                if (jwtConfig is null)
                    throw new Exception(); //todo

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)),
                    ValidateIssuerSigningKey = true,
                };
            });


        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.Read,
                policy => policy.RequireRole(UserType.Admin.ToString(), UserType.Employee.ToString(), UserType.SpecialUser.ToString()))
            .AddPolicy(Policies.Edit, policy => policy.RequireRole(UserType.Admin.ToString(), UserType.Employee.ToString()))
            .AddPolicy(Policies.Admin, policy => policy.RequireRole(UserType.Admin.ToString()));
    }
}

public static class Policies
{
    public const string Admin = "Admin";
    public const string Edit = "Edit";
    public const string Read = "ReadOnly";
}
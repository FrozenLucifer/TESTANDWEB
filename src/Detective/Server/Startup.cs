using System.Text.Json.Serialization;
using DataAccess.Extensions;
using Detective.Extensions;
using Detective.Middlewares;
using Domain.Configurations;
using Domain.Interfaces;
using Domain.Interfaces.Service;
using Logic;
using Logic.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Detective;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        try
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            
            configuration = builder.Build();
            
            Configuration = configuration;
            
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
            
            Log.Information("Application starting up...");
            Log.Debug("Application starting up...");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application startup failed");
            throw;
        }
    }

    public void ConfigureServices(IServiceCollection services)
    {
        try
        {
            Log.Information("Configuring services...");
            services.AddSerilog();
            
            services.AddContext(Configuration.GetConnectionString("DefaultConnection"));
            services.AddScopedRepositories();
            services.ApplyMigrations();

            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPersonService, PersonService>();

            services.AddOptions<JwtConfiguration>().Bind(Configuration.GetSection(JwtConfiguration.ConfigurationSectionName));
            
            services.AddControllers()
                .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

            services.AddAuthentication(Configuration);

            services.AddSwaggerGen();
            
            Log.Information("Services configured successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error configuring services");
            throw;
        }
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        try
        {
            Log.Information("Configuring application...");
            
            app.UseMiddleware<ExceptionMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();    
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Detective API V1"); });
            
            Log.Information("Application configured successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error configuring application");
            throw;
        }
    }
}
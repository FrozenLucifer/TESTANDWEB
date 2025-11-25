using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Detective.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ApplyMigrations(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<Context>();

        dbContext.Database.Migrate();
    }
}
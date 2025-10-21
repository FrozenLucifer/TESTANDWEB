using DataAccess.Repository;
using Domain.Interfaces.Repository;
using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Extensions;

public static class RepositoriesExtension
{
    public static void AddScopedRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IRelationshipRepository, RelationshipRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICharacteristicRepository, CharacteristicRepository>();
    }

    public static void AddContext(this IServiceCollection services, string? connectionString)
    {
        services.AddDbContext<Context>(opt =>
            opt.UseNpgsql(connectionString).UseExceptionProcessor());
    }
}
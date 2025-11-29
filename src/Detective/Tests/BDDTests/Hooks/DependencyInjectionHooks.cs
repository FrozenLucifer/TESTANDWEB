using Domain.Interfaces;
using Logic;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll;
using Reqnroll.BoDi;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace BDDTests.Hooks;

public static class ContainerSetup
{
    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        var services = new ServiceCollection();
        services.AddScoped<IPasswordProvider, PasswordProvider>();
        return services;
    }
}
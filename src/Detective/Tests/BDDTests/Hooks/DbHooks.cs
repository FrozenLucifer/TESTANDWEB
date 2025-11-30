using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reqnroll;

namespace BDDTests.Hooks;

[Binding]
public class DBHooks
{
    public static DbContextOptions<Context> DbOptions = null!;
    public static IConfiguration Config = null!;

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        DbOptions = new DbContextOptionsBuilder<Context>()
            .UseNpgsql(Config.GetConnectionString("DefaultConnection"))
            .Options;
    }
}
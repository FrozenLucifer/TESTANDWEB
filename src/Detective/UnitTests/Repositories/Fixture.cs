using DataAccess;
using EntityFramework.Exceptions.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TestBase;

namespace UnitTests.Repositories;

public class InMemoryDatabaseFixture : DatabaseFixtureBase
{
    private SqliteConnection _connection;

    public override Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<Context>()
            .UseSqlite(_connection)
            .UseExceptionProcessor()
            .Options;

        DbContext = new Context(options);
        
        DbContext.Database.EnsureCreated();

        return Task.CompletedTask;
    }

    public override Task DisposeAsync()
    {
        DbContext.Dispose();
        _connection.Close();
        _connection.Dispose();
        return Task.CompletedTask;
    }
}
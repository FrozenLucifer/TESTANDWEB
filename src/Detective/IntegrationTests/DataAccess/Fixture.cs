﻿using DataAccess;
using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using TestBase;

namespace IntegrationTests.DataAccess;

public class PostgresDatabaseFixture : DatabaseFixtureBase
{
    private string _testDbName;

    public override async Task InitializeAsync()
    {
        _testDbName = $"TestDb_{DateTime.Now:MMddHHmmss}_{Guid.NewGuid():N}";
        var connectionString = $"Host=localhost;Database={_testDbName};Username=postgres;Password=1";

        var options = new DbContextOptionsBuilder<Context>()
            .UseNpgsql(connectionString)
            .UseExceptionProcessor()
            .Options;
        
        DbContext = new Context(options);
        await DbContext.Database.EnsureCreatedAsync();
    }

    public override async Task DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.DisposeAsync();
    }
}
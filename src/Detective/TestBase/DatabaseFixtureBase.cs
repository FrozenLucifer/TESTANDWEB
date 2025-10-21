using DataAccess;
using Xunit;

namespace TestBase;

public abstract class DatabaseFixtureBase : IAsyncLifetime
{
    public Context DbContext { get; protected set; }

    public abstract Task InitializeAsync();
    public abstract Task DisposeAsync();
}
using DataAccess;
using DataAccess.Repository;
using Domain.Exceptions.Repositories;
using Domain.Interfaces.Repository;
using Xunit;

namespace TestBase.Repositories;

public class PropertyRepositoryTests<TFixture>: IAsyncLifetime
    where TFixture : DatabaseFixtureBase, new()
{
    private TFixture _fixture;
    private Context _dbContext;
    private IPersonRepository _personRepository;
    private IPropertyRepository _propertyRepository;
    
    public async Task InitializeAsync()
    {
        _fixture = new TFixture();
        await _fixture.InitializeAsync();

        _dbContext = _fixture.DbContext;
        _personRepository = new PersonRepository(_dbContext);
        _propertyRepository = new PropertyRepository(_dbContext);

        _dbContext.ChangeTracker.Clear();
    }
    
    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task CreateProperty_ShouldCreateSuccessfully()
    {
        var personId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        await _personRepository.CreatePerson(personId, null, null, null);

        await _propertyRepository.CreateProperty(propertyId, personId, "House", 100000);

        var properties = await _propertyRepository.GetPersonProperties(personId);
        Assert.Single(properties);
        var p = properties[0];
        Assert.Equal(propertyId, p.Id);
        Assert.Equal("House", p.Type);
        Assert.Equal(100000, p.Cost);
    }

    [Fact]
    public async Task CreateProperty_ShouldThrow_WhenPersonDoesNotExist()
    {
        var personId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();

        await Assert.ThrowsAsync<PersonNotFoundRepositoryException>(() =>
            _propertyRepository.CreateProperty(propertyId, personId, "House", 100000));
    }

    [Fact]
    public async Task CreateProperty_ShouldThrow_WhenPropertyAlreadyExists()
    {
        var personId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        await _personRepository.CreatePerson(personId, null, null, null);

        await _propertyRepository.CreateProperty(propertyId, personId, "House", 100000);

        await Assert.ThrowsAsync<PropertyAlreadyExistsRepositoryException>(() =>
            _propertyRepository.CreateProperty(propertyId, personId, "Apartment", 200000));
    }

    [Fact]
    public async Task GetPersonProperties_ShouldReturnEmpty_WhenNoProperties()
    {
        var personId = Guid.NewGuid();
        await _personRepository.CreatePerson(personId, null, null, null);

        var properties = await _propertyRepository.GetPersonProperties(personId);
        Assert.Empty(properties);
    }

    [Fact]
    public async Task GetPersonProperties_ShouldThrow_WhenPersonNotFound()
    {
        var personId = Guid.NewGuid();
        await Assert.ThrowsAsync<PersonNotFoundRepositoryException>(() =>
            _propertyRepository.GetPersonProperties(personId));
    }

    [Fact]
    public async Task DeleteProperty_ShouldDeleteSuccessfully()
    {
        var personId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        await _personRepository.CreatePerson(personId, null, null, null);

        await _propertyRepository.CreateProperty(propertyId, personId, "House", 100000);

        await _propertyRepository.DeleteProperty(propertyId);

        var properties = await _propertyRepository.GetPersonProperties(personId);
        Assert.Empty(properties);

        await Assert.ThrowsAsync<PropertyNotFoundRepositoryException>(() =>
            _propertyRepository.DeleteProperty(propertyId));
    }

    [Fact]
    public async Task DeleteProperty_ShouldThrow_WhenPropertyNotFound()
    {
        var propertyId = Guid.NewGuid();
        await Assert.ThrowsAsync<PropertyNotFoundRepositoryException>(() =>
            _propertyRepository.DeleteProperty(propertyId));
    }
}

// public class RandomOrderer : ITestCaseOrderer
// {
//     public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
//         where TTestCase : ITestCase
//     {
//         return testCases.OrderBy(tc => Guid.NewGuid());
//     }
// }
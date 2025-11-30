using DataAccess;
using DataAccess.Repository;
using Domain.Exceptions.Repositories;
using Domain.Interfaces.Repository;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestBase.Repositories;

public class PropertyRepositoryTests<TFixture>
    where TFixture : DatabaseFixtureBase, new()
{
    private readonly Context _dbContext;
    private readonly IPersonRepository _personRepository;
    private readonly IPropertyRepository _propertyRepository;

    public PropertyRepositoryTests()
    {
        var fixture = new TFixture();
        fixture.InitializeAsync().GetAwaiter().GetResult();
        _dbContext = fixture.DbContext;
        _personRepository = new PersonRepository(_dbContext);
        _propertyRepository = new PropertyRepository(_dbContext);

        _dbContext.ChangeTracker.Clear();
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
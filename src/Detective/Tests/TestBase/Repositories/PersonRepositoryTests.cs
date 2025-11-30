using DataAccess;
using DataAccess.Repository;
using Domain.Enums;
using Domain.Exceptions.Repositories;
using Domain.Interfaces.Repository;
using Domain.Models;
using Xunit;

namespace TestBase.Repositories;

public class PersonRepositoryTests<TFixture>
    where TFixture : DatabaseFixtureBase, new()
{
    private readonly Context _dbContext;
    private readonly IPersonRepository _personRepository;
    private readonly PersonFabric _personFabric;

    public PersonRepositoryTests()
    {
        var fixture = new TFixture();
        fixture.InitializeAsync().GetAwaiter().GetResult();
        _dbContext = fixture.DbContext;
        _personRepository = new PersonRepository(_dbContext);
        _personFabric = new PersonFabric();

        _dbContext.ChangeTracker.Clear();
        _dbContext.RemoveRange();
    }

    [Fact]
    public async Task CreatePerson_ShouldThrow_WhenIdAlreadyExists()
    {
        var person = _personFabric.CreatePerson1();
        await _personRepository.CreatePerson(person.Id, person.Sex, person.FullName, person.BirthDate);

        await Assert.ThrowsAsync<PersonAlreadyExistsRepositoryException>(() =>
            _personRepository.CreatePerson(person.Id, person.Sex, person.FullName, person.BirthDate));
    }

    [Fact]
    public async Task CreatePerson_ShouldCreateSuccessfully()
    {
        var person = _personFabric.CreatePerson2();
        await _personRepository.CreatePerson(person.Id, person.Sex, person.FullName, person.BirthDate);

        var loaded = await _personRepository.GetPerson(person.Id);
        Assert.Equal(person.Id, loaded.Id);
        Assert.Equal(person.Sex, loaded.Sex);
        Assert.Equal(person.FullName, loaded.FullName);
        Assert.Equal(person.BirthDate, loaded.BirthDate);
    }

    [Fact]
    public async Task GetPerson_ShouldReturnPerson_WhenExists()
    {
        var person = _personFabric.CreatePerson1();
        await _personRepository.CreatePerson(person.Id, person.Sex, person.FullName, person.BirthDate);

        var loaded = await _personRepository.GetPerson(person.Id);
        Assert.Equal(person.Id, loaded.Id);
        Assert.Equal(person.FullName, loaded.FullName);
    }

    [Fact]
    public async Task GetPerson_ShouldThrow_WhenNotFound()
    {
        await Assert.ThrowsAsync<PersonNotFoundRepositoryException>(() =>
            _personRepository.GetPerson(Guid.NewGuid()));
    }

    [Fact]
    public async Task UpdatePerson_ShouldUpdateSuccessfully()
    {
        var person = _personFabric.CreatePerson1();
        await _personRepository.CreatePerson(person.Id, person.Sex, person.FullName, person.BirthDate);

        await _personRepository.UpdatePerson(person.Id, Sex.Female, "Updated Name", new DateOnly(2000, 1, 1));

        var updated = await _personRepository.GetPerson(person.Id);
        Assert.Equal(Sex.Female, updated.Sex);
        Assert.Equal("Updated Name", updated.FullName);
        Assert.Equal(new DateOnly(2000, 1, 1), updated.BirthDate);
    }

    [Fact]
    public async Task DeletePerson_ShouldDeleteSuccessfully()
    {
        var person = _personFabric.CreatePerson2();
        await _personRepository.CreatePerson(person.Id, person.Sex, person.FullName, person.BirthDate);

        await _personRepository.DeletePerson(person.Id);

        await Assert.ThrowsAsync<PersonNotFoundRepositoryException>(() =>
            _personRepository.GetPerson(person.Id));
    }
} 

public class PersonBuilder
{
    private Guid _id = Guid.NewGuid();
    private Sex _sex = Sex.Male;
    private string _fullName = "Default Name";
    private DateOnly? _birthDate;

    public PersonBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public PersonBuilder WithSex(Sex sex)
    {
        _sex = sex;
        return this;
    }

    public PersonBuilder WithFullName(string name)
    {
        _fullName = name;
        return this;
    }

    public PersonBuilder WithBirthDate(DateOnly? date)
    {
        _birthDate = date;
        return this;
    }

    public Person Build() => new Person(id: _id, sex: _sex, fullName: _fullName, birthDate: _birthDate);
}

public class PersonFabric
{
    public Person CreatePerson1()
    {
        var person = new PersonBuilder()
            .WithFullName("Male 1")
            .WithSex(Sex.Male)
            .WithBirthDate(new DateOnly(1990, 1, 1))
            .Build();

        return person;
    }

    public Person CreatePerson2()
    {
        var person = new PersonBuilder()
            .WithFullName("Female 1")
            .WithSex(Sex.Female)
            .WithBirthDate(new DateOnly(1995, 5, 5))
            .Build();

        return person;
    }
}
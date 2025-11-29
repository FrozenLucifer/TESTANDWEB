using DataAccess;
using DataAccess.Repository;
using Domain.Enums;
using Domain.Exceptions.Repositories;
using Domain.Interfaces.Repository;
using Xunit;

namespace TestBase.Repositories;

public class ContactRepositoryTests<TFixture> : IAsyncLifetime
    where TFixture : DatabaseFixtureBase, new()
{
    private TFixture _fixture;
    private Context _dbContext;
    private ContactRepository _contactRepository;
    private PersonRepository _personRepository { get; set; }

    public async Task InitializeAsync()
    {
        _fixture = new TFixture();
        await _fixture.InitializeAsync();

        _dbContext = _fixture.DbContext;
        _personRepository = new PersonRepository(_dbContext);
        _contactRepository = new ContactRepository(_dbContext);

        _dbContext.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task CreateContact_ShouldThrowException_WhenPersonDoesNotExist()
    {
        var personId = Guid.NewGuid();
        var contactId = Guid.NewGuid();

        await Assert.ThrowsAsync<PersonNotFoundRepositoryException>(() =>
            _contactRepository.CreateContact(contactId, personId, ContactType.Email, "123"));
    }

    [Fact]
    public async Task CreateContact_ShouldThrowException_WhenContactIdAlreadyExists()
    {
        var personId = Guid.NewGuid();
        var contactId = Guid.NewGuid();

        await _personRepository.CreatePerson(personId, null, null, null);
        await _contactRepository.CreateContact(contactId, personId, ContactType.Email, "123");

        await Assert.ThrowsAsync<ContactAlreadyExistsRepositoryException>(() =>
            _contactRepository.CreateContact(contactId, personId, ContactType.Email, "1234"));
    }

    [Fact]
    public async Task CreateContact_ShouldCreateSuccessfully()
    {
        var personId = Guid.NewGuid();
        var contactId = Guid.NewGuid();

        await _personRepository.CreatePerson(personId, null, null, null);
        await _contactRepository.CreateContact(contactId, personId, ContactType.Email, "test@example.com");

        var contacts = await _contactRepository.GetPersonContacts(personId);
        Assert.Single(contacts);
        Assert.Equal(contactId, contacts[0].Id);
        Assert.Equal("test@example.com", contacts[0].Info);
    }

    [Fact]
    public async Task GetPersonContacts_ShouldThrowException_WhenPersonDoesNotExist()
    {
        var personId = Guid.NewGuid();
        await Assert.ThrowsAsync<PersonNotFoundRepositoryException>(() =>
            _contactRepository.GetPersonContacts(personId));
    }

    [Fact]
    public async Task GetPersonContacts_ShouldReturnFilteredContacts()
    {
        var personId = Guid.NewGuid();
        await _personRepository.CreatePerson(personId, null, null, null);

        await _contactRepository.CreateContact(Guid.NewGuid(), personId, ContactType.Email, "email@example.com");
        await _contactRepository.CreateContact(Guid.NewGuid(), personId, ContactType.Phone, "+123456789");

        var emailContacts = await _contactRepository.GetPersonContacts(personId, ContactType.Email);
        Assert.Single(emailContacts);
        Assert.Equal(ContactType.Email, emailContacts[0].Type);
    }

    [Fact]
    public async Task DeleteContact_ShouldThrowException_WhenContactDoesNotExist()
    {
        var contactId = Guid.NewGuid();
        await Assert.ThrowsAsync<ContactNotFoundRepositoryException>(() =>
            _contactRepository.DeleteContact(contactId));
    }

    [Fact]
    public async Task DeleteContact_ShouldDeleteSuccessfully()
    {
        var personId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        await _personRepository.CreatePerson(personId, null, null, null);
        await _contactRepository.CreateContact(contactId, personId, ContactType.Email, "delete@example.com");

        await _contactRepository.DeleteContact(contactId);

        var contacts = await _contactRepository.GetPersonContacts(personId);
        Assert.Empty(contacts);
    }

    [Fact]
    public async Task GetPersonIdByContact_ShouldReturnPerson()
    {
        var personId = Guid.NewGuid();
        var person = await _personRepository.CreatePerson(personId, null, null, null);

        await _contactRepository.CreateContact(Guid.NewGuid(), personId, ContactType.Email, "find@example.com");

        var result = await _contactRepository.GetPersonByContact(ContactType.Email, "find@example.com");
        Assert.Equal(person.Id, result.Id);
    }

    [Fact]
    public async Task GetPersonIdByContact_ShouldThrowException_WhenContactNotFound()
    {
        await Assert.ThrowsAsync<PersonNotFoundRepositoryException>(() =>
            _contactRepository.GetPersonByContact(ContactType.Email, "notfound@example.com"));
    }
}
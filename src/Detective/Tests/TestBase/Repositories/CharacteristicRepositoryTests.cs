using DataAccess;
using DataAccess.Repository;
using Domain.Enums;
using Domain.Exceptions.Repositories;
using Domain.Interfaces.Repository;
using Xunit;

namespace TestBase.Repositories;

public class CharacteristicRepositoryTests<TFixture>: IAsyncLifetime
    where TFixture : DatabaseFixtureBase, new()
{
    private TFixture _fixture;
    private Context _dbContext;
    private IPersonRepository _personRepository;
    private IUserRepository _userRepository;
    private ICharacteristicRepository _characteristicRepository;
    
    
    public async Task InitializeAsync()
    {
        _fixture = new TFixture();
        await _fixture.InitializeAsync();

        _dbContext = _fixture.DbContext;
        _personRepository = new PersonRepository(_dbContext);
        _userRepository = new UserRepository(_dbContext);
        _characteristicRepository = new CharacteristicRepository(_dbContext);

        _dbContext.ChangeTracker.Clear();
    }
    
    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task CreateCharacteristic_ShouldCreateSuccessfully()
    {
        var personId = Guid.NewGuid();
        var characteristicId = Guid.NewGuid();
        await _personRepository.CreatePerson(personId, null, null, null);
        await _userRepository.CreateUser("user1", "", "", UserType.Admin);

        await _characteristicRepository.CreateCharacteristic(
            characteristicId, personId, "user1", "tall", "friendly", "none");

        var characteristics = await _characteristicRepository.GetPersonsCharacteristics(personId);
        Assert.Single(characteristics);
        var c = characteristics[0];
        Assert.Equal(characteristicId, c.Id);
        Assert.Equal("user1", c.AuthorUsername);
        Assert.Equal("tall", c.Appearance);
        Assert.Equal("friendly", c.Personality);
        Assert.Equal("none", c.MedicalConditions);
    }

    [Fact]
    public async Task CreateCharacteristic_ShouldThrow_WhenAuthorDoesNotExist()
    {
        var personId = Guid.NewGuid();
        var characteristicId = Guid.NewGuid();
        await _personRepository.CreatePerson(personId, null, null, null);

        await Assert.ThrowsAsync<UserNotFoundRepositoryException>(() =>
            _characteristicRepository.CreateCharacteristic(
                characteristicId, personId, "nonexistent_user", "tall", "friendly", "none"));
    }

    [Fact]
    public async Task GetCharacteristicById_ShouldReturnCharacteristic()
    {
        var personId = Guid.NewGuid();
        var characteristicId = Guid.NewGuid();
        await _personRepository.CreatePerson(personId, null, null, null);
        await _userRepository.CreateUser("user2", "", "", UserType.Admin);

        await _characteristicRepository.CreateCharacteristic(
            characteristicId, personId, "user2", "short", "shy", "asthma");

        var characteristic = await _characteristicRepository.GetCharacteristicById(characteristicId);
        Assert.Equal(characteristicId, characteristic.Id);
        Assert.Equal("user2", characteristic.AuthorUsername);
    }

    [Fact]
    public async Task GetCharacteristicById_ShouldThrow_WhenNotFound()
    {
        var id = Guid.NewGuid();
        await Assert.ThrowsAsync<CharacteristicNotFoundRepositoryException>(() =>
            _characteristicRepository.GetCharacteristicById(id));
    }

    [Fact]
    public async Task GetPersonsCharacteristics_ShouldReturnEmpty_WhenNoCharacteristics()
    {
        var personId = Guid.NewGuid();
        await _personRepository.CreatePerson(personId, null, null, null);

        var characteristics = await _characteristicRepository.GetPersonsCharacteristics(personId);
        Assert.Empty(characteristics);
    }

    [Fact]
    public async Task DeleteCharacteristic_ShouldDeleteSuccessfully()
    {
        var personId = Guid.NewGuid();
        var characteristicId = Guid.NewGuid();
        await _personRepository.CreatePerson(personId, null, null, null);
        await _userRepository.CreateUser("user3", "", "", UserType.Admin);

        await _characteristicRepository.CreateCharacteristic(
            characteristicId, personId, "user3", "average", "calm", "none");

        await _characteristicRepository.DeleteCharacteristic(characteristicId);

        var characteristics = await _characteristicRepository.GetPersonsCharacteristics(personId);
        Assert.Empty(characteristics);

        await Assert.ThrowsAsync<CharacteristicNotFoundRepositoryException>(() =>
            _characteristicRepository.GetCharacteristicById(characteristicId));
    }

    [Fact]
    public async Task DeleteCharacteristic_ShouldThrow_WhenNotFound()
    {
        var id = Guid.NewGuid();
        await Assert.ThrowsAsync<CharacteristicNotFoundRepositoryException>(() =>
            _characteristicRepository.DeleteCharacteristic(id));
    }
}
using DataAccess;
using DataAccess.Models;
using DataAccess.Repository;
using Domain.Enums;
using Domain.Exceptions.Repositories;
using Domain.Interfaces.Repository;
using Domain.Models;
using Xunit;

namespace TestBase.Repositories;

public class RelationshipRepositoryTests<TFixture>
    where TFixture : DatabaseFixtureBase, new()
{
    private readonly Context _dbContext;
    private readonly IPersonRepository _personRepository;
    private readonly IRelationshipRepository _relationshipRepository;

    public RelationshipRepositoryTests()
    {
        var fixture = new TFixture();
        fixture.InitializeAsync().GetAwaiter().GetResult();
        _dbContext = fixture.DbContext;
        _personRepository = new PersonRepository(_dbContext);
        _relationshipRepository = new RelationshipRepository(_dbContext);

        _dbContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task GetPersonRelationships_ShouldReturnEmpty_WhenNoRelationships()
    {
        // Arrange
        var id = Guid.NewGuid();
        await _personRepository.CreatePerson(id, null, null, null);

        // Act
        var relationships = await _relationshipRepository.GetPersonRelationships(id);

        // Assert
        Assert.Empty(relationships);
    }

    [Fact]
    public async Task GetPersonRelationships_ShouldReturnRelationships()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        await _personRepository.CreatePerson(id1, null, null, null);
        await _personRepository.CreatePerson(id2, null, null, null);

        await _relationshipRepository.SetRelationship(id1, id2, RelationshipType.Friend);

        // Act
        var relationships = await _relationshipRepository.GetPersonRelationships(id1);

        // Assert
        Assert.Single(relationships);
        Assert.Contains(new Relationship(id1, id2, RelationshipType.Friend), relationships);
    }

    [Fact]
    public async Task SetRelationship_ShouldCreateRelationship()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        await _personRepository.CreatePerson(id1, null, null, null);
        await _personRepository.CreatePerson(id2, null, null, null);

        // Act
        await _relationshipRepository.SetRelationship(id1, id2, RelationshipType.Friend);

        // Assert
        var type1 = await _relationshipRepository.GetPersonsRelationship(id1, id2);
        var type2 = await _relationshipRepository.GetPersonsRelationship(id2, id1);

        Assert.Equal(RelationshipType.Friend, type1);
        Assert.Equal(RelationshipHelper.GetInverseRelationship(RelationshipType.Friend), type2);
    }

    [Fact]
    public async Task SetRelationship_ShouldThrow_WhenPersonNotFound()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        // Acr & Assert
        await Assert.ThrowsAsync<PersonNotFoundRepositoryException>(() =>
            _relationshipRepository.SetRelationship(id1, id2, RelationshipType.Friend));
    }

    [Fact]
    public async Task SetRelationship_ShouldDeleteRelationship()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        await _personRepository.CreatePerson(id1, null, null, null);
        await _personRepository.CreatePerson(id2, null, null, null);

        await _relationshipRepository.SetRelationship(id1, id2, RelationshipType.Friend);

        // Act
        await _relationshipRepository.SetRelationship(id1, id2, null);

        // Assert
        var relationships = await _relationshipRepository.GetPersonRelationships(id1);
        Assert.Empty(relationships);

        await Assert.ThrowsAsync<RelationshipNotFoundRepositoryException>(() =>
            _relationshipRepository.GetPersonsRelationship(id1, id2));
    }

    [Fact]
    public async Task SetRelationship_ShouldThrow_WhenDeletingNonExistingRelationship()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        await _personRepository.CreatePerson(id1, null, null, null);
        await _personRepository.CreatePerson(id2, null, null, null);

        // Act & Assert
        await Assert.ThrowsAsync<RelationshipNotFoundRepositoryException>(() =>
            _relationshipRepository.SetRelationship(id1, id2, null));
    }

    [Fact]
    public async Task GetPersonsRelationship_ShouldThrow_WhenNotFound()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        await _personRepository.CreatePerson(id1, null, null, null);
        await _personRepository.CreatePerson(id2, null, null, null);

        // Act & Assert
        await Assert.ThrowsAsync<RelationshipNotFoundRepositoryException>(() =>
            _relationshipRepository.GetPersonsRelationship(id1, id2));
    }
    
    [Fact]
    public async Task SetRelationship_ShouldUpdateExistingRelationship()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        await _personRepository.CreatePerson(id1, null, null, null);
        await _personRepository.CreatePerson(id2, null, null, null);
        
        await _relationshipRepository.SetRelationship(id1, id2, RelationshipType.Friend);

        // Act
        await _relationshipRepository.SetRelationship(id1, id2, RelationshipType.Spouse);

        // Assert
        var updatedType1 = await _relationshipRepository.GetPersonsRelationship(id1, id2);
        var updatedType2 = await _relationshipRepository.GetPersonsRelationship(id2, id1);

        Assert.Equal(RelationshipType.Spouse, updatedType1);
        Assert.Equal(RelationshipHelper.GetInverseRelationship(RelationshipType.Spouse), updatedType2);
    }

}
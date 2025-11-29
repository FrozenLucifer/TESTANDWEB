using DataAccess;
using DataAccess.Repository;
using Domain.Enum;
using Domain.Exceptions.Repositories;
using Domain.Interfaces.Repository;
using Xunit;

namespace TestBase.Repositories;

public class DocumentRepositoryTests<TFixture>
    where TFixture : DatabaseFixtureBase, new()
{
    private readonly Context _dbContext;
    private readonly IPersonRepository _personRepository;
    private readonly IDocumentRepository _documentRepository;

    public DocumentRepositoryTests()
    {
        var fixture = new TFixture();
        fixture.InitializeAsync().GetAwaiter().GetResult();
        _dbContext = fixture.DbContext;
        _personRepository = new PersonRepository(_dbContext);
        _documentRepository = new DocumentRepository(_dbContext);

        _dbContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task CreateDocument_ShouldCreateSuccessfully()
    {
        var personId = Guid.NewGuid();
        var documentId = Guid.NewGuid();
        await _personRepository.CreatePerson(personId, null, null, null);

        await _documentRepository.CreateDocument(documentId, personId, DocumentType.Passport, "{}");

        var documents = await _documentRepository.GetPersonDocuments(personId);
        Assert.Single(documents);
        var d = documents[0];
        Assert.Equal(documentId, d.Id);
        Assert.Equal(DocumentType.Passport, d.Type);
        Assert.Equal("{}", d.Payload);
    }

    [Fact]
    public async Task CreateDocument_ShouldThrow_WhenPersonDoesNotExist()
    {
        var personId = Guid.NewGuid();
        var documentId = Guid.NewGuid();

        await Assert.ThrowsAsync<PersonNotFoundRepositoryException>(() =>
            _documentRepository.CreateDocument(documentId, personId, DocumentType.Passport, "{}"));
    }

    [Fact]
    public async Task CreateDocument_ShouldThrow_WhenDocumentAlreadyExists()
    {
        var personId = Guid.NewGuid();
        var documentId = Guid.NewGuid();
        await _personRepository.CreatePerson(personId, null, null, null);

        await _documentRepository.CreateDocument(documentId, personId, DocumentType.Passport, "{}");

        await Assert.ThrowsAsync<DocumentAlreadyExistsRepositoryException>(() =>
            _documentRepository.CreateDocument(documentId, personId, DocumentType.Passport, "{}"));
    }

    [Fact]
    public async Task GetPersonDocuments_ShouldReturnEmpty_WhenNoDocuments()
    {
        var personId = Guid.NewGuid();
        await _personRepository.CreatePerson(personId, null, null, null);

        var documents = await _documentRepository.GetPersonDocuments(personId);
        Assert.Empty(documents);
    }

    [Fact]
    public async Task GetPersonDocuments_ShouldThrow_WhenPersonNotFound()
    {
        var personId = Guid.NewGuid();
        await Assert.ThrowsAsync<PersonNotFoundRepositoryException>(() =>
            _documentRepository.GetPersonDocuments(personId));
    }

    [Fact]
    public async Task DeleteDocument_ShouldDeleteSuccessfully()
    {
        var personId = Guid.NewGuid();
        var documentId = Guid.NewGuid();
        await _personRepository.CreatePerson(personId, null, null, null);
        await _documentRepository.CreateDocument(documentId, personId, DocumentType.Passport, "{}");

        await _documentRepository.DeleteDocument(documentId);

        var documents = await _documentRepository.GetPersonDocuments(personId);
        Assert.Empty(documents);

        await Assert.ThrowsAsync<DocumentNotFoundRepositoryException>(() =>
            _documentRepository.DeleteDocument(documentId));
    }

    [Fact]
    public async Task DeleteDocument_ShouldThrow_WhenDocumentNotFound()
    {
        var documentId = Guid.NewGuid();
        await Assert.ThrowsAsync<DocumentNotFoundRepositoryException>(() =>
            _documentRepository.DeleteDocument(documentId));
    }
}
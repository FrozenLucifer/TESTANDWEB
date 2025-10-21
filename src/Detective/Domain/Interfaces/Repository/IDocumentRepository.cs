using Domain.Enum;
using Domain.Models;

namespace Domain.Interfaces.Repository;

public interface IDocumentRepository
{
    public Task CreateDocument(Guid id, Guid personId, DocumentType type, string info);

    public Task<List<Document>> GetPersonDocuments(Guid personId);

    public Task DeleteDocument(Guid id);
}
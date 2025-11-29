using DataAccess.Models;
using DataAccess.Models.Converters;
using Domain.Enums;
using Domain.Exceptions.Repositories;
using Domain.Interfaces.Repository;
using Domain.Models;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class DocumentRepository : IDocumentRepository
{
    private readonly Context _context;

    public DocumentRepository(Context context)
    {
        _context = context;
    }

    public async Task CreateDocument(Guid id, Guid personId, DocumentType type, string info)
    {
        try
        {
            var documentDb = new DocumentDb(id, personId, type, info);

            await _context.Documents.AddAsync(documentDb);

            await _context.SaveChangesAsync();
        }
        catch (ReferenceConstraintException ex)
        {
            throw new PersonNotFoundRepositoryException(personId);
        }
        catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
        {
            throw new DocumentAlreadyExistsRepositoryException(id);
        }
    }

    public async Task<List<Document>> GetPersonDocuments(Guid personId)
    {
        var personExists = await _context.Persons.AnyAsync(p => p.Id == personId);
        if (!personExists)
            throw new PersonNotFoundRepositoryException(personId);

        var documents = await _context.Documents
            .AsNoTracking()
            .Where(d => d.PersonId == personId)
            .ToListAsync();

        return documents.ConvertAll(DocumentConverter.ToDomain);
    }

    public async Task DeleteDocument(Guid id)
    {
        var documentDb = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);
        if (documentDb is null)
            throw new DocumentNotFoundRepositoryException(id);

        _context.Documents.Remove(documentDb);
        await _context.SaveChangesAsync();
    }
}
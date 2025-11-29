using DataAccess.Models;
using DataAccess.Models.Converters;
using Domain.Enums;
using Domain.Exceptions.Repositories;
using Domain.Interfaces.Repository;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class RelationshipRepository : IRelationshipRepository
{
    private readonly Context _context;

    public RelationshipRepository(Context context)
    {
        _context = context;
    }

    public async Task SetRelationship(Guid id1, Guid id2, RelationshipType? type)
    {
        if (type is null)
        {
            var relationshipDb1 = await _context.Relationships.FindAsync(id1, id2);
            if (relationshipDb1 is null)
                throw new RelationshipNotFoundRepositoryException(id1, id2);

            var relationshipDb2 = await _context.Relationships.FindAsync(id2, id1);
            if (relationshipDb2 is null)
                throw new RelationshipNotFoundRepositoryException(id2, id1);

            _context.Relationships.Remove(relationshipDb1);
            _context.Relationships.Remove(relationshipDb2);
        }
        else
        {
            var existing1 = await _context.Relationships.FindAsync(id1, id2);
            var existing2 = await _context.Relationships.FindAsync(id2, id1);

            if (existing1 != null && existing2 != null)
            {
                existing1.Type = type.Value;
                existing2.Type = RelationshipHelper.GetInverseRelationship(type.Value);
            }
            else
            {
                var relationshipDb1 = new RelationshipDb(id1, id2, type.Value);
                var relationshipDb2 = new RelationshipDb(id2, id1, RelationshipHelper.GetInverseRelationship(type.Value));
                _context.Relationships.Add(relationshipDb1);
                _context.Relationships.Add(relationshipDb2);
            }
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw new PersonNotFoundRepositoryException();
        }
    }

    public async Task<RelationshipType> GetPersonsRelationship(Guid id1, Guid id2)
    {
        var relationshipDb = await _context.Relationships.FindAsync(id1, id2);
        if (relationshipDb is null)
            throw new RelationshipNotFoundRepositoryException(id1, id2);

        return relationshipDb.Type;
    }

    public async Task<List<Relationship>> GetPersonRelationships(Guid id)
    {
        var personDb = await _context.Persons.Include(p => p.RelationshipsAsPerson1).FirstOrDefaultAsync(p => p.Id == id);

        if (personDb is null)
            throw new PersonNotFoundRepositoryException(id);

        var relationshipsDb = personDb.RelationshipsAsPerson1.ToList();

        return relationshipsDb.ConvertAll(RelationshipConverter.ToDomain);
    }
}
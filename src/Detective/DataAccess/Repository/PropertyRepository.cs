using DataAccess.Models;
using DataAccess.Models.Converters;
using Domain.Exceptions.Repositories;
using Domain.Interfaces.Repository;
using Domain.Models;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class PropertyRepository : IPropertyRepository
{
    private readonly Context _context;

    public PropertyRepository(Context context)
    {
        _context = context;
    }

    public async Task CreateProperty(Guid id, Guid personId, string name, int? cost)
    {
        try
        {
            var propertyDb = new PropertyDb(id, personId, name, cost);

            await _context.Properties.AddAsync(propertyDb);

            await _context.SaveChangesAsync();
        }
        catch (ReferenceConstraintException ex)
        {
            throw new PersonNotFoundRepositoryException(personId);
        }
        catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
        {
            throw new PropertyAlreadyExistsRepositoryException(id);
        }
    }

    public async Task<List<PersonProperty>> GetPersonProperties(Guid personId)
    {
        var personDb = await _context.Persons
            .Include(p => p.Properties)
            .FirstOrDefaultAsync(p => p.Id == personId);

        if (personDb is null)
            throw new PersonNotFoundRepositoryException(personId);

        var propertiesDb = personDb.Properties.ToList();

        return propertiesDb.ConvertAll(PropertyConverter.ToDomain);
    }

    public async Task DeleteProperty(Guid id)
    {
        var personDb = await _context.Properties.FindAsync(id);
        if (personDb is null)
            throw new PropertyNotFoundRepositoryException(id);

        _context.Properties.Remove(personDb);

        await _context.SaveChangesAsync();
    }
}
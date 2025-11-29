using DataAccess.Models;
using DataAccess.Models.Converters;
using Domain.Enums;
using Domain.Exceptions.Repositories;
using Domain.Interfaces.Repository;
using Domain.Models;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class ContactRepository : IContactRepository
{
    private readonly Context _context;

    public ContactRepository(Context context)
    {
        _context = context;
    }

    public async Task CreateContact(Guid id, Guid personId, ContactType type, string info)
    {
        try
        {
            var contactDb = new ContactDb(id, personId, type, info);

            await _context.Contacts.AddAsync(contactDb);

            await _context.SaveChangesAsync();
        }
        catch (ReferenceConstraintException ex)
        {
            throw new PersonNotFoundRepositoryException(personId);
        }
        catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
        {
            throw new ContactAlreadyExistsRepositoryException(id);
        }
    }

    public async Task<List<Contact>> GetPersonContacts(Guid personId, ContactType? type = null)
    {
        var personDb = await _context.Persons
            .Include(p => p.Contacts)
            .FirstOrDefaultAsync(p => p.Id == personId);

        if (personDb is null)
            throw new PersonNotFoundRepositoryException(personId);

        var contactsDb = personDb.Contacts.ToList();

        if (type != null)
        {
            contactsDb = contactsDb.Where(c => c.Type == type).ToList();
        }

        return contactsDb.ConvertAll(ContactConverter.ToDomain);
    }

    public async Task DeleteContact(Guid id)
    {
        var contactDb = await _context.Contacts.FindAsync(id);
        if (contactDb is null)
            throw new ContactNotFoundRepositoryException(id);

        _context.Contacts.Remove(contactDb);
        await _context.SaveChangesAsync();
    }

    public async Task<Person> GetPersonByContact(ContactType contactType, string info)
    {
        var contactDb = await _context.Contacts.Include(c => c.Person).FirstOrDefaultAsync(c => c.Type == contactType && c.Info == info);

        if (contactDb is null)
            throw new PersonNotFoundRepositoryException();

        return contactDb.Person.ToDomain();
    }
}
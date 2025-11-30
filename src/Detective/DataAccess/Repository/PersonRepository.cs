using System.Globalization;
using DataAccess.Models;
using DataAccess.Models.Converters;
using Domain.Enums;
using Domain.Exceptions.Repositories;
using Domain.Interfaces.Repository;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class PersonRepository : IPersonRepository
{
    private readonly Context _context;

    public PersonRepository(Context context)
    {
        _context = context;
    }

    public async Task<Person> CreatePerson(Guid id, Sex? sex, string? fullName, DateOnly? birthDate)
    {
        try
        {
            var personDb = new PersonDb(id, sex, fullName, birthDate);

            await _context.Persons.AddAsync(personDb);

            await _context.SaveChangesAsync();

            return personDb.ToDomain();
        }
        catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
        {
            throw new PersonAlreadyExistsRepositoryException(id);
        }
    }

    public async Task<List<Person>> GetPersons(Sex? sex,
        string? fullName,
        DateOnly? minBirthDate,
        DateOnly? maxBirthDate,
        int? limit,
        int? skip)
    {
        var query = _context.Persons.AsQueryable();

        if (sex.HasValue)
        {
            query = query.Where(p => p.Sex == sex.Value);
        }

        if (minBirthDate.HasValue)
        {
            query = query.Where(p => p.BirthDate >= minBirthDate.Value);
        }

        if (maxBirthDate.HasValue)
        {
            query = query.Where(p => p.BirthDate <= maxBirthDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(fullName))
        {
            var searchTerms = fullName
                .ToLower(CultureInfo.CurrentCulture)
                .Split([' '], StringSplitOptions.RemoveEmptyEntries);

            foreach (var term in searchTerms)
            {
                string pattern = $"%{term}%";
                query = query.Where(p =>
                    p.FullName != null &&
                    EF.Functions.Like(p.FullName.ToLower(), pattern));
            }
        }


        query = query.OrderBy(p => p.FullName);

        if (limit is > 0)
            query = query.Take(limit.Value);

        if (skip is > 0)
            query = query.Skip(skip.Value);

        return (await query.ToListAsync()).ConvertAll(PersonConverter.ToDomain);
    }

    public async Task<Person> GetPerson(Guid id)
    {
        var personDb = await _context.Persons.AsNoTracking().Where(p => p.Id == id).FirstOrDefaultAsync();
        if (personDb is null)
            throw new PersonNotFoundRepositoryException(id);

        return personDb.ToDomain();
    }

    public async Task UpdatePerson(Guid id, Sex? sex, string? fullName, DateOnly? birthDate)
    {
        var personDb = await _context.Persons.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (personDb is null)
            throw new PersonNotFoundRepositoryException(id);

        personDb.Sex = sex;
        personDb.FullName = fullName;
        personDb.BirthDate = birthDate;

        await _context.SaveChangesAsync();
    }

    public async Task DeletePerson(Guid id)
    {
        var personDb = await _context.Persons.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (personDb is null)
            throw new PersonNotFoundRepositoryException(id);

        _context.Persons.Remove(personDb);

        await _context.SaveChangesAsync();
    }
}
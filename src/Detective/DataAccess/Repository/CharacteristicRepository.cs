using DataAccess.Models;
using DataAccess.Models.Converters;
using Domain.Exceptions.Repositories;
using Domain.Interfaces.Repository;
using Domain.Models;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class CharacteristicRepository : ICharacteristicRepository
{
    private readonly Context _context;

    public CharacteristicRepository(Context context)
    {
        _context = context;
    }

    public async Task<Characteristic> GetCharacteristicById(Guid id)
    {
        var characteristicDb = await _context.Characteristics.AsNoTracking().Where(p => p.Id == id).FirstOrDefaultAsync();
        if (characteristicDb is null)
            throw new CharacteristicNotFoundRepositoryException(id);

        return characteristicDb.ToDomain();
    }

    public async Task<List<Characteristic>> GetPersonsCharacteristics(Guid personId)
    {
        var characteristicsDb = await _context.Characteristics
            .AsNoTracking()
            .Where(c => c.PersonId == personId)
            .ToListAsync();

        if (characteristicsDb == null)
            throw new CharacteristicNotFoundRepositoryException(personId);

        return characteristicsDb.ConvertAll(CharacteristicConverter.ToDomain);
    }

    public async Task CreateCharacteristic(Guid id,
        Guid personId,
        string authorUsername,
        string appearance,
        string personality,
        string medicalConditions)
    {
        try
        {
            var characteristic = new CharacteristicDb(
                id,
                personId,
                authorUsername,
                appearance,
                personality,
                medicalConditions
            );

            await _context.Characteristics.AddAsync(characteristic);
            await _context.SaveChangesAsync();
        }
        catch (ReferenceConstraintException ex)
        {
            throw new UserNotFoundRepositoryException(authorUsername);
        }
        catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
        {
            throw new CharacteristicAlreadyExistsRepositoryException(id);
        }

    }

    public async Task DeleteCharacteristic(Guid id)
    {
        var characteristicDb = await _context.Characteristics.Where(p => p.Id == id).FirstOrDefaultAsync();

        if (characteristicDb is null)
            throw new CharacteristicNotFoundRepositoryException(id);

        _context.Characteristics.Remove(characteristicDb);
        await _context.SaveChangesAsync();
    }
}
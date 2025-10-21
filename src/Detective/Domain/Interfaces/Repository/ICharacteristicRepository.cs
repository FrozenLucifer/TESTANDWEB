using Domain.Models;

namespace Domain.Interfaces.Repository;

public interface ICharacteristicRepository
{
    Task<Characteristic> GetCharacteristicById(Guid id);
    Task<List<Characteristic>> GetPersonsCharacteristics(Guid personId);

    Task CreateCharacteristic(Guid id,
        Guid personId,
        string authorUsername,
        string appearance,
        string personality,
        string medicalConditions);

    Task DeleteCharacteristic(Guid id);
}
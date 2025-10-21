using Domain.Models;

namespace DataAccess.Models.Converters;

public static class CharacteristicConverter
{
    public static Characteristic ToDomain(this CharacteristicDb characteristic)
    {
        return new Characteristic(characteristic.Id,
            characteristic.PersonId,
            characteristic.AuthorUsername,
            characteristic.Appearance,
            characteristic.Personality,
            characteristic.MedicalConditions);
    }
}
using Domain.Models;

namespace DataAccess.Models.Converters;

public static class PropertyConverter
{
    public static PersonProperty ToDomain(this PropertyDb property)
    {
        return new PersonProperty(property.Id, property.PersonId, property.Name, property.Cost);
    }
}
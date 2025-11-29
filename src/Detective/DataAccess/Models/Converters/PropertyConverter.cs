using Domain.Models;

namespace DataAccess.Models.Converters;

public static class PropertyConverter
{
    public static Property ToDomain(this PropertyDb property)
    {
        return new Property(property.Id, property.PersonId, property.Name, property.Cost);
    }
}
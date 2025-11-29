using Domain.Models;

namespace DataAccess.Models.Converters;

public static class DocumentConverter
{
    public static Document ToDomain(this DocumentDb property)
    {
        return new Document(property.Id, property.PersonId, property.Type, property.Payload);
    }
}
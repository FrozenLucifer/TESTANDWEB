using Domain.Models;

namespace DataAccess.Models.Converters;

public static class ContactConverter
{
    public static Contact ToDomain(this ContactDb contact)
    {
        return new Contact(contact.Id, contact.PersonId, contact.Type, contact.Info);
    }
}
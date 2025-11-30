using Domain.Enums;
using Domain.Models;
using DTOs;
using DTOs.Enum;

namespace Detective.Dtos.Converters;

public static class ContactDtoConverter
{
    public static ContactDto ToDto(this Contact contact)
    {
        return new ContactDto
        {
            Id = contact.Id,
            PersonId = contact.PersonId,
            Type = contact.Type.ToDto(),
            Info = contact.Info
        };
    }
}


public static class ContactTypeConverter
{
    public static ContactType ToDomain(this ContactTypeDto dtoType)
    {
        return (ContactType)Enum.Parse(typeof(ContactType), dtoType.ToString());
    }

    public static ContactTypeDto ToDto(this ContactType domainType)
    {
        return (ContactTypeDto)Enum.Parse(typeof(ContactTypeDto), domainType.ToString());
    }
}

public static class SexConverter
{
    public static Sex? ToDomain(this SexDto? dtoType)
    {
        if (dtoType is null)
            return null;

        return (Sex)Enum.Parse(typeof(SexDto), dtoType.ToString());
    }

    public static SexDto? ToDto(this Sex? domainType)
    {
        if (domainType is null)
            return null;

        return (SexDto)Enum.Parse(typeof(SexDto), domainType.ToString());
    }
}

public static class DocumentTypeConverter
{
    public static DocumentType ToDomain(this DocumentTypeDto dtoType)
    {
        return (DocumentType)Enum.Parse(typeof(DocumentTypeDto), dtoType.ToString());
    }

    public static DocumentTypeDto ToDto(this DocumentType domainType)
    {
        return (DocumentTypeDto)Enum.Parse(typeof(DocumentTypeDto), domainType.ToString());
    }
}

public static class RelationshipTypeConverter
{
    public static RelationshipType ToDomain(this RelationshipTypeDto dtoType)
    {
        return (RelationshipType)Enum.Parse(typeof(RelationshipTypeDto), dtoType.ToString());
    }

    public static RelationshipTypeDto ToDto(this RelationshipType domainType)
    {
        return (RelationshipTypeDto)Enum.Parse(typeof(RelationshipTypeDto), domainType.ToString());
    }
}

public static class UserTypeConverter
{
    public static UserType ToDomain(this UserTypeDto dtoType)
    {
        return (UserType)Enum.Parse(typeof(UserTypeDto), dtoType.ToString());
    }

    public static UserTypeDto ToDto(this UserType domainType)
    {
        return (UserTypeDto)Enum.Parse(typeof(UserTypeDto), domainType.ToString());
    }
}



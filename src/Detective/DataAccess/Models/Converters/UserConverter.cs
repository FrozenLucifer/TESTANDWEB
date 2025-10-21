using Domain.Models;

namespace DataAccess.Models.Converters;

public static class UserConverter
{
    public static User ToDomain(this UserDb user)
    {
        return new User(user.Type, user.Username, user.Password);
    }
}

public static class RelationshipConverter
{
    public static Relationship ToDomain(this RelationshipDb relationship)
    {
        return new Relationship(relationship.Person1Id, relationship.Person2Id, relationship.Type);
    }
}
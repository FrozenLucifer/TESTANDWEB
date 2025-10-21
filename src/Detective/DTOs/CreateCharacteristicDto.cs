using Domain.Enum;
using DTOs.Enum;

namespace DTOs;

public class CreateCharacteristicDto
{
    public string Appearance { get; set; }
    public string Personality { get; set; }
    public string MedicalConditions { get; set; }
}

public class ConnectPersonsDto
{
    public Guid person1Id { get; set; }
    public Guid person2Id { get; set; }
    public RelationshipTypeDto type { get; set; }
}

public class DeleteRelationshipDto
{
    public Guid person1Id { get; set; }
    public Guid person2Id { get; set; }
}

public class AddPersonContactDto
{
    public ContactTypeDto type { get; set; }
    public string info { get; set; }
}

public class AddPersonPropertyDto
{
    public string name { get; set; }
    public int? cost { get; set; }
}

public class CreateUserDto
{
    public string username { get; set; }
    public UserType type { get; set; }
}
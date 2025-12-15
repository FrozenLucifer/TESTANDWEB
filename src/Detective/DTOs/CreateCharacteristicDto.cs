using DTOs.Enums;

namespace DTOs;

public class CreateCharacteristicDto
{
    public string Appearance { get; set; }
    public string Personality { get; set; }
    public string MedicalConditions { get; set; }
}

public class ConnectPersonsDto
{
    public Guid Person1Id { get; set; }
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
    public ContactTypeDto Type { get; set; }
    public string Info { get; set; }
}

public class AddPersonPropertyDto
{
    public string Name { get; set; }
    public int? Cost { get; set; }
}

public class CreateUserDto
{
    public string Username { get; set; }
    public UserTypeDto Type { get; set; }
}
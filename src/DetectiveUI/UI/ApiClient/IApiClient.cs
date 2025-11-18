using DTOs;
using DTOs.Enum;


namespace DetectiveUI.ApiClient;

public interface IApiClient
{
    Task<LoginResponseDto> LoginAsync(string username, string password);

    Task ChangePasswordAsync(string username, string oldPassword, string newPassword);

    Task<List<PersonDto>> GetPersonsAsync(GetPersonFilterDto filter);

    Task<Guid> CreatePersonAsync(CreatePersonDto create);

    Task<PersonFullDto> GetPersonInfoAsync(Guid personId);

    Task DeletePersonAsync(Guid personId);

    Task ChangePersonGeneralInfoAsync(Guid personId, SexDto? sex, string? fullName, DateOnly? birthDate);

    Task<List<RelationshipDto>> GetPersonRelationshipsAsync(Guid personId, uint depth = 1);

    Task ConnectPersonsAsync(Guid person1Id, Guid person2Id, RelationshipTypeDto type);

    Task DeleteRelationship(Guid person1Id, Guid person2Id);

    Task AddPersonContactAsync(Guid personId, ContactTypeDto type, string info);

    Task DeletePersonContactAsync(Guid contactId);

    Task<Guid> GetPersonIdByContactAsync(ContactTypeDto type, string info);

    Task AddPersonPassportAsync(Guid personId, PassportPayloadDto info);

    Task DeletePersonDocumentAsync(Guid documentId);

    Task AddPersonPropertyAsync(Guid personId, string name, int? cost);

    Task DeletePersonPropertyAsync(Guid propertyId);

    Task<string> CreateUserAsync(string username, UserTypeDto type);

    Task DeleteUserAsync(string username);

    Task<string> ResetPasswordAsync(string username);

    Task<List<UserDto>> GetUsersAsync();

    public Task<Guid> AddCharacteristicAsync(Guid personId, CreateCharacteristicDto create);

    public Task DeletePersonCharacteristicAsync(Guid propertyId);

}
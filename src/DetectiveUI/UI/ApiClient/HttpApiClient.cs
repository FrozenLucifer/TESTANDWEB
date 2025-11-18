using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using DTOs;
using DTOs.Enum;

namespace DetectiveUI.ApiClient;

public class ApiException : Exception
{
    public HttpStatusCode StatusCode;

    public ApiException(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }
}

public class HttpApiClient : IApiClient
{
    private readonly RestClient _restClient;
    private readonly TokenService _tokenService;

    public HttpApiClient(RestClient restClient, TokenService tokenService)
    {
        _restClient = restClient;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> LoginAsync(string username, string password)
    {
        var request = new RestRequest("api/v1/auth/login", Method.Post);
        request.AddJsonBody(new LoginRequestDto(username, password));

        var response = await _restClient.ExecuteAsync<LoginResponseDto>(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }

        return response.Data;
    }

    public async Task ChangePasswordAsync(string username, string oldPassword, string newPassword)
    {
        var request = new RestRequest("api/v1/auth/password", Method.Patch);
        request.AddJsonBody(new ChangePasswordRequestDto(username, oldPassword, newPassword));

        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task<List<PersonDto>> GetPersonsAsync(GetPersonFilterDto filter)
    {
        var request = new RestRequest("api/v1/persons");
        await _tokenService.AddAuthHeader(request);
        request.AddBody(filter);

        var response = await _restClient.ExecuteAsync<List<PersonDto>>(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }

        return response.Data;
    }

    public async Task<Guid> CreatePersonAsync(CreatePersonDto create)
    {
        var request = new RestRequest("api/v1/persons", Method.Post);
        await _tokenService.AddAuthHeader(request);

        request.AddBody(create);

        var response = await _restClient.ExecuteAsync<Guid>(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }

        return response.Data;
    }

    public async Task<PersonFullDto> GetPersonInfoAsync(Guid personId)
    {
        var request = new RestRequest($"api/v1/persons/{personId}");
        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync<PersonFullDto>(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }

        return response.Data;
    }

    public async Task DeletePersonAsync(Guid personId)
    {
        var request = new RestRequest($"api/v1/persons/{personId}", Method.Delete);
        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task ChangePersonGeneralInfoAsync(Guid personId, SexDto? sex, string? fullName, DateOnly? birthDate)
    {
        var request = new RestRequest($"api/v1/persons/{personId}", Method.Patch);
        await _tokenService.AddAuthHeader(request);

        var x = new CreatePersonDto();

        if (sex.HasValue) x.Sex = sex.Value;
        if (!string.IsNullOrEmpty(fullName)) x.FullName = fullName;
        if (birthDate.HasValue) x.BirthDate = birthDate.Value;

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task<List<RelationshipDto>> GetPersonRelationshipsAsync(Guid personId, uint depth = 1)
    {
        var request = new RestRequest($"api/v1/persons/{personId}/relationships");
        await _tokenService.AddAuthHeader(request);

        request.AddQueryParameter("depth", depth.ToString());

        var response = await _restClient.ExecuteAsync<List<RelationshipDto>>(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }

        return response.Data;
    }

    public async Task ConnectPersonsAsync(Guid person1Id, Guid person2Id, RelationshipTypeDto type)
    {
        var request = new RestRequest("api/v1/persons/relationships", Method.Post);
        await _tokenService.AddAuthHeader(request);

        request.AddBody(new RelationshipDto { Person1Id = person1Id, Person2Id = person2Id, Type = type });

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task DeleteRelationship(Guid person1Id, Guid person2Id)
    {
        var request = new RestRequest("api/v1/persons/relationships", Method.Delete);
        await _tokenService.AddAuthHeader(request);

        request.AddBody(new DeleteRelationshipDto { person1Id = person1Id, person2Id = person2Id });

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task AddPersonContactAsync(Guid personId, ContactTypeDto type, string info)
    {
        var request = new RestRequest($"api/v1/persons/{personId}/contacts", Method.Post);
        await _tokenService.AddAuthHeader(request);

        request.AddBody(new AddPersonContactDto { info = info, type = type });

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task DeletePersonContactAsync(Guid contactId)
    {
        var request = new RestRequest($"api/v1/persons/contacts/{contactId}", Method.Delete);
        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task<Guid> GetPersonIdByContactAsync(ContactTypeDto type, string info)
    {
        throw new NotImplementedException();
    }

    public async Task AddPersonPassportAsync(Guid personId, PassportPayloadDto info)
    {
        var request = new RestRequest($"api/v1/persons/{personId}/documents", Method.Post);
        await _tokenService.AddAuthHeader(request);

        request.AddJsonBody(info);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task DeletePersonDocumentAsync(Guid documentId)
    {
        var request = new RestRequest($"api/v1/persons/documents/{documentId}", Method.Delete);
        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task AddPersonPropertyAsync(Guid personId, string name, int? cost)
    {
        var request = new RestRequest($"api/v1/persons/{personId}/property", Method.Post);
        await _tokenService.AddAuthHeader(request);
        request.AddBody(new AddPersonPropertyDto { name = name, cost = cost });

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task DeletePersonPropertyAsync(Guid propertyId)
    {
        var request = new RestRequest($"api/v1/persons/property/{propertyId}", Method.Delete);
        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task<Guid> AddCharacteristicAsync(Guid personId, CreateCharacteristicDto create)
    {
        var request = new RestRequest($"api/v1/persons/{personId}/characteristic", Method.Post);
        await _tokenService.AddAuthHeader(request);

        request.AddBody(create);

        var response = await _restClient.ExecuteAsync<Guid>(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }

        return response.Data;
    }


    public async Task DeletePersonCharacteristicAsync(Guid propertyId)
    {
        var request = new RestRequest($"api/v1/persons/characteristic/{propertyId}", Method.Delete);
        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task<string> CreateUserAsync(string username, UserTypeDto type)
    {
        var request = new RestRequest("api/v1/users", Method.Post);
        await _tokenService.AddAuthHeader(request);

        request.AddBody(new CreateUserDto { username = username, type = type });

        var response = await _restClient.ExecuteAsync<string>(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }

        return response.Data;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var request = new RestRequest("api/v1/users");
        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync<List<UserDto>>(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }

        return response.Data;
    }

    public async Task DeleteUserAsync(string username)
    {
        var request = new RestRequest($"api/v1/users/{username}", Method.Delete);
        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task<string> ResetPasswordAsync(string username)
    {
        var request = new RestRequest($"api/v1/users/{username}/reset", Method.Put);
        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync<string>(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }

        return response.Data;
    }


}

public class TokenService
{
    private string? _token;
    private JwtSecurityToken? _parsedToken;

    public void SetToken(string token)
    {
        _token = token;
        _parsedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
    }

    public void ClearToken()
    {
        _token = null;
        _parsedToken = null;
    }

    public JwtSecurityToken GetParsedToken()
    {
        return _parsedToken;
    }

    public Task AddAuthHeader(RestRequest request)
    {
        if (!string.IsNullOrEmpty(_token))
        {
            request.AddHeader("Authorization", $"Bearer {_token}");
        }

        return Task.CompletedTask;
    }

    public string GetRoleClaim()
    {
        if (_parsedToken == null) return string.Empty;

        var roleClaim = _parsedToken.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Role);

        return roleClaim?.Value ?? string.Empty;
    }
}
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
        var request = new RestRequest("api/auth", Method.Post);
        request.AddJsonBody(new { username, password });

        var response = await _restClient.ExecuteAsync<LoginResponseDto>(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }

        return response.Data;
    }

    public async Task ChangePasswordAsync(string username, string oldPassword, string newPassword)
    {
        var request = new RestRequest("api/auth", Method.Patch);
        request.AddJsonBody(new { username, oldPassword, newPassword });

        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task<List<PersonDto>> GetPersonsAsync(GetPersonFilterDto filter)
    {
        var request = new RestRequest("api/persons", Method.Get);
        await _tokenService.AddAuthHeader(request);

        request.AddQueryParameter("sex", filter.Sex?.ToString());
        request.AddQueryParameter("fullName", filter.FullName);
        request.AddQueryParameter("minBirthDate", filter.MinBirthDate?.ToString("yyyy-MM-dd"));
        request.AddQueryParameter("maxBirthDate", filter.MaxBirthDate?.ToString("yyyy-MM-dd"));
        request.AddQueryParameter("limit", filter.Limit?.ToString());

        var response = await _restClient.ExecuteAsync<List<PersonDto>>(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }

        return response.Data;
    }

    public async Task<Guid> CreatePersonAsync(CreatePersonDto create)
    {
        var request = new RestRequest("api/persons", Method.Post);
        await _tokenService.AddAuthHeader(request);

        request.AddQueryParameter("sex", create.Sex.ToString());
        request.AddQueryParameter("fullName", create.FullName);
        request.AddQueryParameter("birthDate", create.BirthDate.ToString());

        var response = await _restClient.ExecuteAsync<Guid>(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }

        return response.Data;
    }
    
    
    public async Task<Guid> AddCharacteristicAsync(CreateCharacteristicDto create)
    {
        var request = new RestRequest("api/persons/characteristic", Method.Post);
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
        var request = new RestRequest($"api/persons/{personId}", Method.Get);
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
        var request = new RestRequest($"api/persons/{personId}", Method.Delete);
        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task ChangePersonGeneralInfoAsync(Guid personId, SexDto? sex, string? fullName, DateOnly? birthDate)
    {
        var request = new RestRequest($"api/persons/{personId}", Method.Patch);
        await _tokenService.AddAuthHeader(request);

        if (sex.HasValue) request.AddQueryParameter("sex", sex.Value.ToString());
        if (!string.IsNullOrEmpty(fullName)) request.AddQueryParameter("fullName", fullName);
        if (birthDate.HasValue) request.AddQueryParameter("birthDate", birthDate.Value.ToString("yyyy-MM-dd"));

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task<List<RelationshipDto>> GetPersonRelationshipsAsync(Guid personId, uint depth = 1)
    {
        var request = new RestRequest($"api/persons/relationships/{personId}", Method.Get);
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
        var request = new RestRequest("api/persons/relationships", Method.Post);
        await _tokenService.AddAuthHeader(request);

        request.AddQueryParameter("person1Id", person1Id.ToString());
        request.AddQueryParameter("person2Id", person2Id.ToString());
        request.AddQueryParameter("type", type.ToString());

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task DeleteRelationship(Guid person1Id, Guid person2Id)
    {
        var request = new RestRequest("api/persons/relationships", Method.Delete);
        await _tokenService.AddAuthHeader(request);

        request.AddQueryParameter("person1Id", person1Id.ToString());
        request.AddQueryParameter("person2Id", person2Id.ToString());

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task AddPersonContactAsync(Guid personId, ContactTypeDto type, string info)
    {
        var request = new RestRequest("api/persons/contacts", Method.Post);
        await _tokenService.AddAuthHeader(request);

        request.AddQueryParameter("personId", personId.ToString());
        request.AddQueryParameter("type", type.ToString());
        request.AddQueryParameter("info", info);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task DeletePersonContactAsync(Guid contactId)
    {
        var request = new RestRequest($"api/persons/contacts/{contactId}", Method.Delete);
        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task<Guid> GetPersonIdByContactAsync(ContactTypeDto type, string info)
    {
        var request = new RestRequest("api/persons/contacts", Method.Get);
        await _tokenService.AddAuthHeader(request);

        request.AddQueryParameter("type", type.ToString());
        request.AddQueryParameter("info", info);

        var response = await _restClient.ExecuteAsync<Guid>(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }

        return response.Data;
    }

    public async Task AddPersonPassportAsync(Guid personId, PassportPayloadDto info)
    {
        var request = new RestRequest("api/persons/documents/passport", Method.Post);
        await _tokenService.AddAuthHeader(request);

        request.AddQueryParameter("personId", personId.ToString());
        request.AddJsonBody(info);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task DeletePersonDocumentAsync(Guid documentId)
    {
        var request = new RestRequest($"api/persons/documents/{documentId}", Method.Delete);
        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task AddPersonPropertyAsync(Guid personId, string name, int? cost)
    {
        var request = new RestRequest("api/persons/property", Method.Post);
        await _tokenService.AddAuthHeader(request);

        request.AddQueryParameter("personId", personId.ToString());
        request.AddQueryParameter("name", name);
        if (cost.HasValue) request.AddQueryParameter("cost", cost.Value.ToString());

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task DeletePersonPropertyAsync(Guid propertyId)
    {
        var request = new RestRequest($"api/persons/property/{propertyId}", Method.Delete);
        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }
    
    public async Task DeletePersonCharacteristicAsync(Guid propertyId)
    {
        var request = new RestRequest($"api/persons/characteristic/{propertyId}", Method.Delete);
        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task<string> CreateUserAsync(string username, UserTypeDto type)
    {
        var request = new RestRequest("api/users", Method.Post);
        await _tokenService.AddAuthHeader(request);

        request.AddQueryParameter("username", username);
        request.AddQueryParameter("type", type.ToString());

        var response = await _restClient.ExecuteAsync<string>(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }

        return response.Data;
    }

    public async Task DeleteUserAsync(string username)
    {
        var request = new RestRequest("api/users", Method.Delete);
        await _tokenService.AddAuthHeader(request);

        request.AddQueryParameter("username", username);

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }
    }

    public async Task<string> ResetPasswordAsync(string username)
    {
        var request = new RestRequest("api/users/reset", Method.Post);
        await _tokenService.AddAuthHeader(request);

        request.AddQueryParameter("username", username);

        var response = await _restClient.ExecuteAsync<string>(request);

        if (!response.IsSuccessful)
        {
            throw new ApiException(response.StatusCode);
        }

        return response.Data;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var request = new RestRequest("api/users", Method.Get);
        await _tokenService.AddAuthHeader(request);

        var response = await _restClient.ExecuteAsync<List<UserDto>>(request);

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
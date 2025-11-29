using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Detective.Dtos.Converters;
using Detective.Extensions;
using Domain.Enum;
using Domain.Interfaces.Service;
using Domain.Models;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Detective.Controllers;

[ApiController]
[Route("api/v1/persons")]
[Authorize]
[Produces("application/json")]
public class PersonController : ControllerBase
{
    private readonly IPersonService _personService;

    public PersonController(IPersonService personService)
    {
        _personService = personService;
    }

    /// <summary>
    /// Получить людей по фильтрам
    /// </summary>
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<PersonDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    [Authorize(Policy = Policies.Read)]
    public async Task<ActionResult<List<PersonDto>>> GetPersons([FromQuery] GetPersonFilterDto filter)
    {
        if (filter.ContactType != null)
        {
            var result = await _personService.GetPersonByContact(filter.ContactType.Value.ToDomain(), filter.ContactInfo);
            var x = new List<Person> { result };
            return Ok(x.ConvertAll(PersonDtoConverter.ToDto));
        }
        else
        {
            var result = await _personService.GetPersons(filter.Sex.ToDomain(),
                filter.FullName,
                filter.MinBirthDate,
                filter.MaxBirthDate,
                filter.Limit,
                filter.Skip);

            return Ok(result.ConvertAll(PersonDtoConverter.ToDto));
        }
    }

    /// <summary>
    /// Создать человека
    /// </summary>
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(Guid))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    [Authorize(Policy = Policies.Edit)]
    public async Task<ActionResult<Guid>> CreatePersons([FromBody] CreatePersonDto create)
    {
        if (create is null)
            throw new ArgumentException(null, nameof(create));
        var result = await _personService.CreatePerson(create.Sex.ToDomain(), create.FullName, create.BirthDate);
        return Ok(result);
    }

    /// <summary>
    /// Получить полную информацию о человеке
    /// </summary>
    [HttpGet("{personId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(PersonFullDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    [Authorize(Policy = Policies.Read)]
    public async Task<ActionResult<PersonFullDto>> GetPersonInfo(Guid personId)
    {
        var result = await _personService.GetPerson(personId);
        return Ok(result.ToDto());
    }

    /// <summary>
    /// Удалить человека по Id
    /// </summary>
    [HttpDelete("{personId:guid}")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    [Authorize(Policy = Policies.Edit)]
    public async Task<ActionResult> DeletePerson(Guid personId)
    {
        await _personService.DeletePerson(personId);
        return NoContent();
    }

    /// <summary>
    /// Изменить основную информацию о человеке
    /// </summary>
    [HttpPatch("{personId:guid}")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    [Authorize(Policy = Policies.Edit)]
    public async Task<ActionResult> ChangePersonGeneralInfo([Required] Guid personId, CreatePersonDto createPersonDto)
    {
        await _personService.ChangePersonGeneralInfo(personId, createPersonDto.Sex.ToDomain(), createPersonDto.FullName, createPersonDto.BirthDate);
        return NoContent();
    }

    /// <summary>
    /// Получить отношения человека
    /// </summary>
    [HttpGet("{personId:guid}/relationships")]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<RelationshipDto>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    [Authorize(Policy = Policies.Read)]
    public async Task<ActionResult<List<RelationshipDto>>> GetPersonRelationships(Guid personId, uint depth = 1)
    {
        var result = await _personService.GetPersonRelationships(personId, depth);
        return Ok(result.ConvertAll(RelationshipDtoConverter.ToDto));
    }

    /// <summary>
    /// Создать связь между людьми
    /// </summary>
    [HttpPost("relationships")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    [Authorize(Policy = Policies.Edit)]
    public async Task<ActionResult> ConnectPersons(ConnectPersonsDto connect)
    {
        await _personService.SetPersonsRelationship(connect.person1Id, connect.person2Id, connect.type.ToDomain());
        return NoContent();
    }

    /// <summary>
    /// Удалить связь между людьми
    /// </summary>
    [HttpDelete("relationships")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    [Authorize(Policy = Policies.Edit)]
    public async Task<ActionResult> DeleteRelationship([Required] DeleteRelationshipDto delete)
    {
        await _personService.DeleteRelationship(delete.person1Id, delete.person2Id);
        return NoContent();
    }

    /// <summary>
    /// Добавить человеку контакт
    /// </summary>
    [HttpPost("{personId}/contacts")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    [Authorize(Policy = Policies.Edit)]
    public async Task<ActionResult> AddPersonContact([FromRoute][Required] Guid personId, AddPersonContactDto addPersonContactDto)
    {
        await _personService.AddPersonContact(personId, addPersonContactDto.type.ToDomain(), addPersonContactDto.info);
        return NoContent();
    }

    /// <summary>
    /// Удалить контакт 
    /// </summary>
    [HttpDelete("contacts/{contactId:guid}")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    [Authorize(Policy = Policies.Edit)]
    public async Task<ActionResult> DeletePersonContact([Required] Guid contactId)
    {
        await _personService.DeletePersonContact(contactId);
        return NoContent();
    }

    /// <summary>
    /// Добавить человеку документ 
    /// </summary>
    [HttpPost("{personId}/documents")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status409Conflict)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    [Authorize(Policy = Policies.Edit)]
    public async Task<ActionResult> AddPersonPassport([Required][FromRoute] Guid personId, [Required][FromBody] PassportPayloadDto info)
    {
        await _personService.AddPersonDocument(personId, DocumentType.Passport, JsonSerializer.Serialize(info));
        return NoContent();
    }


    /// <summary>
    /// Удалить документ 
    /// </summary>
    [HttpDelete("documents/{documentId:guid}")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    [Authorize(Policy = Policies.Edit)]
    public async Task<ActionResult> DeletePersonPassport([Required] Guid documentId)
    {
        await _personService.DeletePersonDocument(documentId);
        return NoContent();
    }

    /// <summary>
    /// Добавить человеку собственность
    /// </summary>
    [HttpPost("{personId}/property")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    [Authorize(Policy = Policies.Edit)]
    public async Task<ActionResult> AddPersonProperty([Required] Guid personId, [Required] AddPersonPropertyDto addPersonPropertyDto)
    {
        await _personService.AddPersonProperty(personId, addPersonPropertyDto.name, addPersonPropertyDto.cost);
        return NoContent();
    }

    /// <summary>
    /// Удалить собственность 
    /// </summary>
    [HttpDelete("property/{propertyId:guid}")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    [Authorize(Policy = Policies.Edit)]
    public async Task<ActionResult> DeletePersonProperty([Required] Guid propertyId)
    {
        await _personService.DeletePersonProperty(propertyId);
        return NoContent();
    }


    /// <summary>
    /// Добавить человеку характеристику
    /// </summary>
    [HttpPost("{personId}/characteristic")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    public async Task<IActionResult> CreateCharacteristic([FromRoute] Guid personId, [Required][FromBody] CreateCharacteristicDto createDto)
    {
        var username = HttpContext.GetUsername();
        var id = await _personService.CreateCharacteristic(personId,
            username,
            createDto.Appearance,
            createDto.Personality,
            createDto.MedicalConditions);

        return Ok(id);
    }

    /// <summary>
    /// Удалить характеристику
    /// </summary>
    [HttpDelete("characteristic/{characteristicId:guid}")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    public async Task<IActionResult> DeleteCharacteristic([Required] Guid characteristicId)
    {
        await _personService.DeleteCharacteristic(characteristicId);
        return NoContent();
    }
}
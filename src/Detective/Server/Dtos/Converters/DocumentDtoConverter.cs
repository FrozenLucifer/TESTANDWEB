using Domain.Models;
using DTOs;

namespace Detective.Dtos.Converters;

public static class DocumentDtoConverter
{
    public static DocumentDto ToDto(this Document document)
    {
        return new DocumentDto
        {
            Id = document.Id,
            PersonId = document.PersonId,
            Type = document.Type.ToDto(),
            Payload = document.Payload
        };
    }
}
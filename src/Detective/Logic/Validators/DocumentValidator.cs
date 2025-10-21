using System.Text.Json;
using Domain.Enum;
using Domain.Models;
using FluentValidation;


namespace Logic.Validators;

public class DocumentValidator : AbstractValidator<(DocumentType type, string payload)>
{
    public DocumentValidator()
    {
        RuleFor(x => x.type).IsInEnum().WithMessage("Недопустимый тип документа");

        RuleFor(x => x.payload)
            .NotEmpty()
            .WithMessage("Информация документа не может быть пустой")
            .Must(BeValidJson)
            .WithMessage("Информация должна быть валидным JSON")
            .DependentRules(() =>
            {
                When(x => x.type == DocumentType.Passport,
                    () =>
                    {
                        RuleFor(x => x.payload)
                            .Must(BeValidPassportJson)
                            .WithMessage(x =>
                            {
                                var (isValid, errors) = ValidatePassportPayload(x.payload);
                                return $"Неверная структура данных паспорта. {string.Join(" ", errors)}";
                            });
                    });
            });
    }

    private static bool BeValidJson(string json)
    {
        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static (bool isValid, List<string> errors) ValidatePassportPayload(string json)
    {
        var errors = new List<string>();

        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var passport = JsonSerializer.Deserialize<PassportPayload>(json, options);

            if (passport == null)
            {
                errors.Add("Не удалось десериализовать данные паспорта");
                return (false, errors);
            }

            var validator = new PassportPayloadValidator();
            var validationResult = validator.Validate(passport);

            if (!validationResult.IsValid)
            {
                errors.AddRange(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            return (validationResult.IsValid, errors);
        }
        catch (Exception ex)
        {
            errors.Add($"Ошибка при обработке данных: {ex.Message}");
            return (false, errors);
        }
    }

    private static bool BeValidPassportJson(string json)
    {
        var (isValid, _) = ValidatePassportPayload(json);
        return isValid;
    }
}

public class PassportPayloadValidator : AbstractValidator<PassportPayload>
{
    public PassportPayloadValidator()
    {
        RuleFor(x => x.Series)
            .NotEmpty()
            .WithMessage("Серия паспорта обязательна")
            .Length(4)
            .WithMessage("Серия паспорта должна содержать 4 цифры")
            .Matches("^[0-9]+$")
            .WithMessage("Серия паспорта должна содержать только цифры");

        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage("Номер паспорта обязателен")
            .Length(6)
            .WithMessage("Номер паспорта должен содержать 6 цифр")
            .Matches("^[0-9]+$")
            .WithMessage("Номер паспорта должен содержать только цифры");

        When(x => !string.IsNullOrEmpty(x.IssuedBy),
            () =>
            {
                RuleFor(x => x.IssuedBy)
                    .Matches(@"^[а-яА-ЯёЁ\s\.]+$")
                    .WithMessage("Поле 'Кем выдан' должно содержать только русские буквы, пробелы и точки");
            });

        When(x => x.IssueDate.HasValue,
            () =>
            {
                RuleFor(x => x.IssueDate)
                    .NotEmpty()
                    .WithMessage("Дата выдачи обязательна")
                    .LessThanOrEqualTo(DateTime.Today)
                    .WithMessage("Дата выдачи не может быть в будущем");
            });


        When(x => x.BirthDate.HasValue,
            () =>
            {
                RuleFor(x => x.BirthDate)
                    .LessThanOrEqualTo(DateTime.Today)
                    .WithMessage("Дата рождения не может быть в будущем");
            });
    }
}
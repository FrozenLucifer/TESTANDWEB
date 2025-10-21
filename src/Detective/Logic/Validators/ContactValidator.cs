using Domain.Enum;
using FluentValidation;
using FluentValidation.Validators;

namespace Logic.Validators;

public class ContactValidator : AbstractValidator<(ContactType type, string info)>
{
    public ContactValidator()
    {
        RuleFor(x => x.type).IsInEnum().WithMessage("Недопустимый тип контакта");
        RuleFor(x => x.info)
            .NotEmpty()
            .WithMessage("Контакт не может быть пустым")
            .DependentRules(() =>
            {
                When(x => x.type == ContactType.Email,
                    () =>
                    {
                        RuleFor(x => x.info)
                            .EmailAddress(EmailValidationMode.Net4xRegex)
                            .WithMessage("Некорректный формат email");
                    });

                When(x => x.type == ContactType.Telegram,
                    () =>
                    {
                        RuleFor(x => x.info)
                            .Matches(@"^@[a-zA-Z0-9_]{5,32}$")
                            .WithMessage("Некорректный Telegram username. Должен начинаться с @ и содержать 5-32 символов (a-z, 0-9, _)");
                    });

                When(x => x.type == ContactType.Vk,
                    () =>
                    {
                        RuleFor(x => x.info)
                            .Matches(@"^@[a-zA-Z0-9_]{5,32}$")
                            .WithMessage("Некорректный VK username. Должен начинаться с @ и содержать 5-32 символов (a-z, 0-9, _)");
                    });

                When(x => x.type == ContactType.Phone,
                    () =>
                    {
                        RuleFor(x => x.info)
                            .Matches(@"^\+?[0-9\s\-\(\)]{11}$")
                            .WithMessage("Некорректный формат телефона. Должен содержать 11 цифр, может начинаться с +");
                    });
            });
    }
}
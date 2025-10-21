using Domain.Enum;
using FluentValidation;

namespace Logic.Validators;

public class PersonValidator : AbstractValidator<(Sex? sex, string? fullName, DateOnly? birthDate)>
{
    public PersonValidator()
    {
        RuleFor(x => x.sex)
            .Must(sex => sex == null || Enum.IsDefined(typeof(Sex), sex.Value))
            .WithMessage("Указан недопустимый пол");

        When(x => x.fullName != null, () =>
        {
            RuleFor(x => x.fullName)
                .NotEmpty()
                .WithMessage("ФИО не может быть пустым")
                .Length(2, 100)
                .WithMessage("ФИО должно быть от 2 до 100 символов")
                .Matches(@"^[a-zA-Zа-яА-Я\s\-]+$")
                .WithMessage("ФИО содержит недопустимые символы");
        });

        RuleFor(x => x.birthDate)
            .Must(birthDate => birthDate == null || birthDate <= DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Дата рождения не может быть в будущем");
    }
}
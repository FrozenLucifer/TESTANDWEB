using FluentValidation;

namespace Logic.Validators;

public class PropertyValidator : AbstractValidator<(string name, long? cost)>
{
    public PropertyValidator()
    {
        RuleFor(x => x.name)
            .NotEmpty()
            .WithMessage("Имя не может быть пустым")
            .Length(2, 100)
            .WithMessage("Имя должно быть от 2 до 100 символов")
            .Matches(@"^[a-zA-Zа-яА-Я0-9\s\-_]+$")
            .WithMessage("Имя содержит недопустимые символы");

        RuleFor(x => x.cost)
            .Must(cost => cost == null || cost <= 1_000_000_000)
            .WithMessage("Стоимость должна быть меньше 1 000 000 000 или null");
    }
}
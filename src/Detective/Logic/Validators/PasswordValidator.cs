using FluentValidation;

namespace Logic.Validators;

public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
        RuleFor(password => password)
            .NotEmpty().WithMessage("Пароль не может быть пустым")
            .MinimumLength(8).WithMessage("Пароль должен содержать минимум 8 символов")
            .MaximumLength(32).WithMessage("Пароль не должен превышать 32 символа")
            .Matches("[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
            .Matches("[a-z]").WithMessage("Пароль должен содержать хотя бы одну строчную букву")
            .Matches("[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру")
            .Must(NotContainWhitespace).WithMessage("Пароль не должен содержать пробелы");
    }

    private bool NotContainWhitespace(string password)
    {
        return !string.IsNullOrWhiteSpace(password) && !password.Contains(" ");
    }
}
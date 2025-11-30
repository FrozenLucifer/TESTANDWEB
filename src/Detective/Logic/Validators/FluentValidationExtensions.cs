using FluentValidation;
using FluentValidation.Results;

namespace Logic.Validators;

public static class FluentValidationExtensions
{
    public static void ThrowIfInvalid(this ValidationResult validationResult)
    {
        if (!validationResult.IsValid)
        {
            throw new ValidationException(
                string.Join(Environment.NewLine,
                    validationResult.Errors.Select(e => e.ErrorMessage)));
        }
    }

    public static string GetErrorMessages(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            return string.Empty;
        }

        return string.Join(Environment.NewLine,
            validationResult.Errors.Select(e => e.ErrorMessage));
    }
}
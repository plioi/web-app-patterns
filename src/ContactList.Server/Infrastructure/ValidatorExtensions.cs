using FluentValidation;
using FluentValidation.Results;

namespace ContactList.Server.Infrastructure;

public class FailedValidationException : Exception
{
    public FailedValidationException(ValidationResult result)
        => Result = result;

    public ValidationResult Result { get; }
}

public static class ValidatorExtensions
{
    public static async Task GuardAsync<TRequest>(this IValidator<TRequest> validator, TRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new FailedValidationException(validationResult);
    }
}

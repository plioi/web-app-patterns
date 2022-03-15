using FluentValidation;
using FluentValidation.Results;

namespace ContactList.Server.Infrastructure;

public static class ValidatorExtensions
{
    public static async Task<IResult> GuardAsync<TRequest, TResponse>(this IValidator<TRequest> validator, TRequest request, Func<TResponse> operation)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.ValidationProblem(ToDictionary(validationResult));

        var response = operation();

        return Results.Ok(response);
    }

    public static async Task<IResult> GuardAsync<TRequest>(this IValidator<TRequest> validator, TRequest request, Action operation)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.ValidationProblem(ToDictionary(validationResult));

        operation();

        return Results.Ok();
    }

    static Dictionary<string, string[]> ToDictionary(ValidationResult validationResult)
        => validationResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray());
}

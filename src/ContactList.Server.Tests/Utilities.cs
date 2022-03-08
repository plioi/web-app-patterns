using System.Text.Json;
using ContactList.Server.Tests.Execution;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace ContactList.Server.Tests;

static class Utilities
{
    public static string Json(object? value) =>
        JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            WriteIndented = true
        });

    public static ValidationResult Validation<TResult>(IRequest<TResult> message)
    {
        using var scope = ServerTestExecution.CreateScope();

        var validator = Validator(scope.ServiceProvider, message);

        if (validator == null)
            throw new InvalidOperationException($"There is no validator for {message.GetType()} messages.");

        return validator.Validate(new ValidationContext<object>(message));
    }

    static void Validate<TResponse>(IServiceProvider serviceProvider, IRequest<TResponse> message)
    {
        var validator = Validator(serviceProvider, message);
        if (validator != null)
        {
            var context = new ValidationContext<object>(message);
            validator.Validate(context).ShouldBeSuccessful();
        }
    }

    static IValidator? Validator<TResult>(IServiceProvider serviceProvider, IRequest<TResult> message)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(message.GetType());
        return serviceProvider.GetService(validatorType) as IValidator;
    }
}

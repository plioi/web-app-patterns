using System.Text.Json;
using ContactList.Server.Model;
using ContactList.Server.Tests.Execution;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContactList.Server.Tests;

static class Utilities
{
    public static string Json(object? value) =>
        JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            WriteIndented = true
        });

    public static async Task ExecuteScopeAsync(Action<IServiceProvider> action)
    {
        await ExecuteScopeAsync(serviceProvider =>
        {
            action(serviceProvider);
            return Task.CompletedTask;
        });
    }

    public static async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        using var scope = ServerTestExecution.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<Database>();

        try
        {
            await database.BeginTransactionAsync();

            await action(scope.ServiceProvider);

            await database.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await database.RollbackTransactionAsync();
            throw;
        }
    }

    public static async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        using var scope = ServerTestExecution.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<Database>();

        try
        {
            await database.BeginTransactionAsync();

            var result = await action(scope.ServiceProvider);

            await database.CommitTransactionAsync();

            return result;
        }
        catch (Exception)
        {
            await database.RollbackTransactionAsync();
            throw;
        }
    }

    public static Task TransactionAsync(Func<Database, Task> action)
        => ExecuteScopeAsync(services => action(services.GetRequiredService<Database>()));

    public static Task<TResult> QueryAsync<TResult>(Func<Database, Task<TResult>> query)
        => ExecuteScopeAsync(services => query(services.GetRequiredService<Database>()));

    public static Task<TEntity?> FindAsync<TEntity>(Guid id) where TEntity : Entity
        => QueryAsync(database => database.Set<TEntity>().FindAsync(id).AsTask());

    public static Task<int> CountAsync<TEntity>() where TEntity : Entity
        => QueryAsync(database => database.Set<TEntity>().CountAsync());

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

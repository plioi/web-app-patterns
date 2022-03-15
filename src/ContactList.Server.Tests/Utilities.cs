using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using ContactList.Contracts;
using ContactList.Server.Model;
using ContactList.Server.Tests.Execution;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static System.Environment;

namespace ContactList.Server.Tests;

static class Utilities
{
    static readonly Random Random = new();

    static string SampleString([CallerMemberName] string caller = "")
        => caller.Replace("Sample", "") + "-" + Guid.NewGuid();

    public static string SampleEmail() => SampleString() + "@example.com";
    public static string SampleName() => SampleString();
    public static string SamplePhoneNumber() => $"{Random.Next(100, 1000)}-555-0{Random.Next(100, 200)}";

    public static async Task<Contact> AddSampleContactAsync(Action<AddContactCommand>? customize = null)
    {
        var command = new AddContactCommand
        {
            Email = SampleEmail(),
            Name = SampleName(),
            PhoneNumber = SamplePhoneNumber()
        };

        customize?.Invoke(command);

        var contactId = (await PostAsync("/api/contacts/add", command)).ContactId;

        var contact = await FindAsync<Contact>(contactId);

        contact.ShouldNotBeNull();

        return contact;
    }

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

    public static async Task<TResponse> GetAsync<TResponse>(string route, IRequest<TResponse> query)
        where TResponse : class
    {
        using var scope = ServerTestExecution.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<ServerApplicationFactory>();
        var client = factory.CreateClient();

        var typedResponse = await client.GetFromJsonAsync<TResponse>(AppendQueryString(route, query));

        if (typedResponse == null)
            throw new Exception("HTTP response body was unexpectedly null.");

        return typedResponse;
    }

    public static async Task<TResponse> PostAsync<TResponse>(string route, IRequest<TResponse> command)
    {
        using var scope = ServerTestExecution.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<ServerApplicationFactory>();
        var client = factory.CreateClient();

        var content = new StringContent(JsonSerializer.Serialize(command, command.GetType()), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(route, content);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var errors = await ReadValidationMessagesAsync(response);

            if (errors.Count > 0)
            {
                var indentedErrorMessages = errors
                    .SelectMany(x => x.Value)
                    .OrderBy(errorMessage => errorMessage)
                    .Select(errorMessage => "    " + errorMessage)
                    .ToArray();

                var actual = string.Join(NewLine, indentedErrorMessages);

                throw new Exception($"Expected no validation errors, but found {indentedErrorMessages.Length}:{NewLine}{actual}");
            }
        }

        response.EnsureSuccessStatusCode();

        TResponse? typedResponse = default;

        if (typeof(TResponse) != typeof(Unit))
            typedResponse = await response.Content.ReadFromJsonAsync<TResponse>();

        if (typedResponse == null)
            throw new Exception("HTTP response body was unexpectedly null.");

        return typedResponse;
    }

    static async Task<Dictionary<string, List<string>>> ReadValidationMessagesAsync(HttpResponseMessage response)
    {
        var parsedResponse = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();

        return parsedResponse?.Errors ?? new Dictionary<string, List<string>>();
    }

    class ValidationFailureResponse
    {
        public string? Title { get; set; }
        public Dictionary<string, List<string>>? Errors { get; set; }
    }

    static string AppendQueryString<TResponse>(string route, IRequest<TResponse> query)
    {
        var queryString = System.Web.HttpUtility.ParseQueryString("");

        var properties = query.GetType().GetProperties();

        foreach (var parameter in properties)
        {
            var value = parameter.GetValue(query);

            if (value != null)
                queryString.Add(parameter.Name, value.ToString());
        }

        var uri = route;

        if (queryString.Count > 0)
            uri += "?" + queryString;

        return uri;
    }

    public static async Task<ValidationResult> ValidationAsync<TResult>(IRequest<TResult> message)
    {
        using var scope = ServerTestExecution.CreateScope();

        var validator = Validator(scope.ServiceProvider, message);

        if (validator == null)
            throw new InvalidOperationException($"There is no validator for {message.GetType()} messages.");

        return await validator.ValidateAsync(new ValidationContext<object>(message));
    }

    static IValidator? Validator<TResult>(IServiceProvider serviceProvider, IRequest<TResult> message)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(message.GetType());
        return serviceProvider.GetService(validatorType) as IValidator;
    }
}

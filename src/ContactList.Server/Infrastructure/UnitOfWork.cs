using ContactList.Server.Model;
using FluentValidation.Results;

namespace ContactList.Server.Infrastructure;

class UnitOfWork
{
    readonly RequestDelegate _next;

    public UnitOfWork(RequestDelegate next)
        => _next = next;

    public async Task InvokeAsync(HttpContext httpContext, Database database)
    {
        try
        {
            await database.BeginTransactionAsync();

            try
            {
                await _next(httpContext);
                await database.CommitTransactionAsync();
            }
            catch (FailedValidationException exception)
            {
                await database.RollbackTransactionAsync();

                var result = Results.ValidationProblem(ToDictionary(exception.Result));

                await result.ExecuteAsync(httpContext);
            }
        }
        catch (Exception)
        {
            await database.RollbackTransactionAsync();
            throw;
        }
    }

    static Dictionary<string, string[]> ToDictionary(ValidationResult validationResult)
        => validationResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray());
}

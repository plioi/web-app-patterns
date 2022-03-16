using ContactList.Server.Model;

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
            await _next(httpContext);
            await database.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await database.RollbackTransactionAsync();
            throw;
        }
    }
}

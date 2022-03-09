using Microsoft.AspNetCore.Mvc.Filters;
using ContactList.Server.Model;

namespace ContactList.Server.Infrastructure;

class UnitOfWork : IAsyncActionFilter
{
    readonly Database _database;

    public UnitOfWork(Database database)
        => _database = database;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await _database.BeginTransactionAsync();

        var actionExecuted = await next();

        if (actionExecuted.Exception != null && !actionExecuted.ExceptionHandled)
            await _database.RollbackTransactionAsync();
        else
            await _database.CommitTransactionAsync();
    }
}

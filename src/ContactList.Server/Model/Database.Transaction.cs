using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ContactList.Server.Model;

public partial class Database
{
    IDbContextTransaction? _currentTransaction;

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null)
            return;

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();

            if (_currentTransaction != null)
                await _currentTransaction.CommitAsync();
        }
        catch
        {
            if (_currentTransaction != null)
                await _currentTransaction.RollbackAsync();

            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_currentTransaction != null)
                await _currentTransaction.RollbackAsync();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}

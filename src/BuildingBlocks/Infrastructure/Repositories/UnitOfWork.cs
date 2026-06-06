using Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Storage;
using SharedKernel.Interface;

namespace Infrastructure.Repositories;

public class UnitOfWork(BaseDbContext context) : IUnitOfWork,IScopedDependency
{

    private IDbContextTransaction? _transaction;

    //private IRepository<User>? _user;




    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
            return;

        _transaction = await context.Database.BeginTransactionAsync(ct);
    }

    public async Task<bool> CommitAsync(CancellationToken ct = default)
    {
        try
        {
            await context.SaveChangesAsync(ct);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(ct);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        catch
        {
            await RollbackAsync();
            return false;
        }
        return true;

    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }


    public void Dispose()
    {
        _transaction?.Dispose();
        context.Dispose();
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await context.SaveChangesAsync(ct);
}
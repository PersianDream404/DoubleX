namespace Infrastructure.Repositories;

using Ardalis.Result;
using global::Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Constants;
using SharedKernel.Helper;
using SharedKernel.Interface.Repositories;
using System;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class QueryRepository<T> : IQueryRepository<T> where T : class
{
    protected readonly BaseDbContext _context;

    public QueryRepository(BaseDbContext context)
    {
        _context = context;
    }

    public IQueryable<T> AsQueryable() =>
        _context.Set<T>().AsNoTracking().AsQueryable();

    public async Task<List<T>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Set<T>().AsNoTracking().ToListAsync(ct);

    public async Task<List<T>> GetFilterAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        await _context.Set<T>().AsNoTracking().Where(predicate).ToListAsync(ct);

    public async Task<T?> GetFilterFirstAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>().AsNoTracking();

        foreach (var include in includes)
            query = query.Include(include);

        return await query.FirstOrDefaultAsync(predicate, ct);
    }

    public async Task<bool> GetAnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        await _context.Set<T>().AnyAsync(predicate, ct);

    public async Task<T?> GetByIdAsync(long id, CancellationToken ct = default) =>
        await _context.Set<T>().FindAsync(new object[] { id }, ct);

    public T? GetById(long id) =>
        _context.Set<T>().AsNoTracking()
            .FirstOrDefault(e => EF.Property<long>(e, "Id") == id);

    public async Task<T?> GetByIdAsync(long id, CancellationToken ct = default, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>().AsNoTracking();

        foreach (var include in includes)
            query = query.Include(include);

        return await query.FirstOrDefaultAsync(e => EF.Property<long>(e, "Id") == id, ct);
    }

    public async Task<bool> IsUniqueAsync(
                Expression<Func<T, bool>> predicate,
                bool isCreate = true,
                CancellationToken ct = default
                )
                    {
                        if (isCreate)
                        {
                
                            return !await _context.Set<T>().AnyAsync(predicate, ct);
                        }
                        else
                        {
                            return await _context.Set<T>().CountAsync(predicate, ct) <= 1;
                        }
                    }

    public async Task<Result> IsAny(int id, string entityname, CancellationToken ct = default)
    {
        var exists = id != 0 && await GetAnyAsync(x => EF.Property<int>(x, "Id") == id, ct);

        return exists
            ? Result.Success()
            : Result.Error(MessageHelper.Format(AppMessages.NotFound, entityname));
    }
    public async Task<Result> IsAny(int[] ids, string entityname, CancellationToken ct = default)
    {
        foreach (var id in ids)
        {
            var exists = id != 0 && await GetAnyAsync(x => EF.Property<int>(x, "Id") == id, ct);

            if (!exists)
                return Result.Error(MessageHelper.Format(AppMessages.NotFound, entityname));
        }
        return Result.Success();
    }
}


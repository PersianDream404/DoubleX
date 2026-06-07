namespace Infrastructure.Repositories;

using global::Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Interface.Repositories;
using System;
using System.Linq.Expressions;

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

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _context.Set<T>().FindAsync(new object[] { id }, ct);

    public T? GetById(int id) =>
        _context.Set<T>().AsNoTracking()
            .FirstOrDefault(e => EF.Property<int>(e, "Id") == id);

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>().AsNoTracking();

        foreach (var include in includes)
            query = query.Include(include);

        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, ct);
    }
}


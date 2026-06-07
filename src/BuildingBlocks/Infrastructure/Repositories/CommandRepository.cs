namespace Infrastructure.Repositories;

using global::Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Base;
using SharedKernel.Interface.Repositories;
using System;
using System.Linq.Expressions;


public class CommandRepository<T> : ICommandRepository<T> where T : class
{
    protected readonly BaseDbContext _context;

    public CommandRepository(BaseDbContext context)
    {
        _context = context;
    }

    public async Task<T> GetOrAddAsync(Expression<Func<T, bool>> predicate, Func<T> create, CancellationToken ct = default)
    {
        var existing = await _context.Set<T>().FirstOrDefaultAsync(predicate, ct);
        if (existing != null)
            return existing;

        var entity = create();
        await AddAsync(entity, ct);
        return entity;
    }

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _context.Set<T>().AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AddRangeAsync(ICollection<T> entities, CancellationToken ct = default)
    {
        await _context.Set<T>().AddRangeAsync(entities, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AttachAsync(T entity, CancellationToken ct = default)
    {
        _context.Set<T>().Attach(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _context.Set<T>().FindAsync(new object[] { id }, ct);
        if (entity == null) return;

        if (entity is BaseEntity softEntity)
        {
            softEntity.Deleted = true;
            _context.Set<T>().Update(entity);
        }
        else
        {
            _context.Set<T>().Remove(entity);
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(CancellationToken ct = default)
    {
        _context.Set<T>().RemoveRange(_context.Set<T>());
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteRangeAsync(ICollection<T> entities, CancellationToken ct = default)
    {
        foreach (var entity in entities)
        {
            if (entity is BaseEntity softEntity)
            {
                softEntity.Deleted = true;
                _context.Set<T>().Update(entity);
            }
            else
            {
                _context.Set<T>().Remove(entity);
            }
        }

        await _context.SaveChangesAsync(ct);
    }
}


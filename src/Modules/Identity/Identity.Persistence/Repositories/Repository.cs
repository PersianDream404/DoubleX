namespace Infrastructure.Repositories;

using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Base;
using SharedKernel.Interface;
using System;
using System.Linq.Expressions;

public class Repository<T> : IRepository<T>
    where T : class
{

    protected readonly IdentityDbContext _context;
    //private readonly DbSet<T> _dbSet;

    public Repository(IdentityDbContext context)
    {
        _context = context;
        //_dbSet = context.Set<T>();
    }
    /// <summary>
    /// دسترسی به داده‌ها به‌صورت IQueryable
    /// </summary>
    public IQueryable<T> AsQueryable() => _context.Set<T>().AsQueryable();

    /// <summary>
    /// دریافت همه رکوردها
    /// </summary>
    public async Task<List<T>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Set<T>().ToListAsync(ct);

    /// <summary>
    /// دریافت رکوردها بر اساس شرط مشخص‌شده
    /// </summary>
    public async Task<List<T>> GetFilterAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        await _context.Set<T>().Where(predicate).ToListAsync(ct);

    public async Task<T?> GetFilterFirstAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default, params Expression<Func<T, object>>[] includes) =>
      await _context.Set<T>().FirstOrDefaultAsync(predicate, ct);

    public async Task<bool> GetAnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
    await _context.Set<T>().AnyAsync(predicate, ct);

    /// <summary>
    /// دریافت رکورد بر اساس کلید اصلی
    /// </summary>
    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _context.Set<T>().FindAsync(new object[] { id }, ct);


    public  T? GetById(int id, CancellationToken ct = default) =>
     _context.Set<T>().FirstOrDefault(e => EF.Property<int>(e, "Id") == id);
    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>();

        foreach (var include in includes)
            query = query.Include(include);

        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, ct);
    }

    /// <summary>
    /// دریافت رکورد بر اساس شرط و در صورت نبود، ایجاد آن
    /// </summary>
    public async Task<T> GetOrAddAsync(Expression<Func<T, bool>> predicate, Func<T> create, CancellationToken ct = default)
    {
        var existing = await _context.Set<T>().FirstOrDefaultAsync(predicate, ct);
        if (existing != null)
            return existing;

        var entity = create();
        await AddAsync(entity, ct);
        return entity;
    }

    /// <summary>
    /// افزودن یک رکورد جدید
    /// </summary>
    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _context.Set<T>().AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// افزودن مجموعه‌ای از رکوردها
    /// </summary>
    public async Task AddRangeAsync(ICollection<T> entities, CancellationToken ct = default)
    {
        await _context.Set<T>().AddRangeAsync(entities, ct);
        await _context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// به‌روزرسانی یک رکورد موجود
    /// </summary>
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

    /// <summary>
    /// حذف یک رکورد بر اساس کلید
    /// </summary>
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
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

    /// <summary>
    /// حذف همه رکوردها از جدول
    /// </summary>
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

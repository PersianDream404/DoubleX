using System.Linq.Expressions;

namespace SharedKernel.Interface;

/// <summary>
/// اینترفیس مخصوص عملیات EF Core.
/// شامل متدهایی با استفاده از Expression.
/// </summary>
using System.Linq.Expressions;

public interface IRepository<T> where T : class
{
    IQueryable<T> AsQueryable();

    Task<List<T>> GetAllAsync(CancellationToken ct = default);
    Task<List<T>> GetFilterAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<T?> GetFilterFirstAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default, params Expression<Func<T, object>>[] includes);
    Task<bool> GetAnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    public T? GetById(int id, CancellationToken ct = default);
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default, params Expression<Func<T, object>>[] includes);

    Task<T> GetOrAddAsync(Expression<Func<T, bool>> predicate, Func<T> create, CancellationToken ct = default);

    Task AddAsync(T entity, CancellationToken ct = default);
    Task AddRangeAsync(ICollection<T> entities, CancellationToken ct = default);

    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task AttachAsync(T entity, CancellationToken ct = default);

    Task DeleteAsync(CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task DeleteRangeAsync(ICollection<T> entities, CancellationToken ct = default);
}

namespace SharedKernel.Interface.Repositories;

using System.Linq.Expressions;

public interface IQueryRepository<T> where T : class
{
    IQueryable<T> AsQueryable();

    Task<List<T>> GetAllAsync(CancellationToken ct = default);

    Task<List<T>> GetFilterAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default);

    Task<T?> GetFilterFirstAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default,
        params Expression<Func<T, object>>[] includes);

    Task<bool> GetAnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default);

    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);

    T? GetById(int id);

    Task<T?> GetByIdAsync(
        int id,
        CancellationToken ct = default,
        params Expression<Func<T, object>>[] includes);
}

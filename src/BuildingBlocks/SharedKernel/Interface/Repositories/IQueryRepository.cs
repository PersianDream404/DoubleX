namespace SharedKernel.Interface.Repositories;

using Ardalis.Result;
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

    Task<T?> GetByIdAsync(long id, CancellationToken ct = default);

    T? GetById(long id);

    Task<T?> GetByIdAsync(
        long id,
        CancellationToken ct = default,
        params Expression<Func<T, object>>[] includes);

    Task<bool> IsUniqueAsync(
    Expression<Func<T, bool>> predicate,
    bool isCreate = true,
    CancellationToken ct = default
);
    Task<Result> IsAny(int id, string entityname, CancellationToken ct = default);
    Task<Result> IsAny(int[] ids, string entityname, CancellationToken ct = default);
}

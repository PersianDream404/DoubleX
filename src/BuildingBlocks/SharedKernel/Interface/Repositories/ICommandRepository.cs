namespace SharedKernel.Interface.Repositories;

using System.Linq.Expressions;




public interface ICommandRepository<T> where T : class
{
    Task<T> GetOrAddAsync(
        Expression<Func<T, bool>> predicate,
        Func<T> create,
        CancellationToken ct = default);

    Task AddAsync(T entity, CancellationToken ct = default);

    Task AddRangeAsync(ICollection<T> entities, CancellationToken ct = default);

    Task UpdateAsync(T entity, CancellationToken ct = default);

    Task AttachAsync(T entity, CancellationToken ct = default);

    Task DeleteAsync(int id, CancellationToken ct = default);

    Task DeleteAsync(CancellationToken ct = default);

    Task DeleteRangeAsync(ICollection<T> entities, CancellationToken ct = default);
}

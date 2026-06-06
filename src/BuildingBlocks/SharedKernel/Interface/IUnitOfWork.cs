namespace SharedKernel.Interface;


public interface IUnitOfWork
{
    //IRepository<User> Users { get; }




    Task BeginTransactionAsync(CancellationToken ct = default);
    Task<bool> CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

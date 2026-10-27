using Modules.CashRegister.Domain.Entities;
using CashRegisterEntity = Modules.CashRegister.Domain.Entities.CashRegister;

namespace Modules.CashRegister.Application.Contract.Interfaces;

public interface ICashRegisterCommandRepository
{
    Task<bool> AddRegisterAsync(CashRegisterEntity register, CancellationToken cancellationToken);
    Task<bool> IsCodeUsedAsync(long salonId, string code, long? exceptId, CancellationToken cancellationToken);
    Task<CashRegisterEntity?> GetRegisterAsync(long id, long salonId, CancellationToken cancellationToken);
    Task<CashRegisterSession?> GetOpenSessionAsync(long cashRegisterId, long salonId, CancellationToken cancellationToken);
    Task<CashRegisterSession?> GetSessionAsync(long sessionId, long salonId, CancellationToken cancellationToken);
    Task<bool> IsTransactionIdempotencyKeyUsedAsync(long salonId, string key, CancellationToken cancellationToken);
    Task<bool> AddSessionWithOpeningTransactionAsync(
        CashRegisterSession session,
        CancellationToken cancellationToken);
    Task AddTransactionAsync(CashRegisterTransaction transaction, CancellationToken cancellationToken);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
    Task<decimal> GetExpectedBalanceAsync(long sessionId, CancellationToken cancellationToken);
}

using Framwork.PagedList;
using Modules.CashRegister.Application.Contract.DTOs.CashRegisters;
using Modules.CashRegister.Application.Contract.DTOs.Sessions;
using Modules.CashRegister.Application.Contract.DTOs.Transactions;
using Modules.CashRegister.Domain.Entities;

namespace Modules.CashRegister.Application.Contract.Interfaces;

public interface ICashRegisterQueryRepository
{
    Task<PagedList<CashRegisterDto>> GetRegistersAsync(
        CashRegisterListRequestDto request,
        CancellationToken cancellationToken);
    Task<CashRegisterDto?> GetRegisterAsync(long id, long salonId, CancellationToken cancellationToken);
    Task<CashRegisterSessionDto?> GetOpenSessionAsync(long cashRegisterId, long salonId, CancellationToken cancellationToken);
    Task<CashRegisterSessionDto?> GetSessionAsync(long sessionId, long salonId, CancellationToken cancellationToken);
    Task<PagedList<CashRegisterSessionDto>> GetSessionsAsync(
        long cashRegisterId,
        long salonId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
    Task<PagedList<CashRegisterTransactionDto>> GetTransactionsAsync(
        long cashRegisterId,
        CashRegisterTransactionListRequestDto request,
        CancellationToken cancellationToken);
    Task<CashRegisterBalanceDto?> GetBalanceAsync(long cashRegisterId, long salonId, CancellationToken cancellationToken);
    Task<CashRegisterTransaction?> GetTransactionByIdempotencyKeyAsync(
        long salonId,
        string idempotencyKey,
        CancellationToken cancellationToken);
}

using Framwork.PagedList;
using Modules.CashRegister.Domain.Entities;

namespace Modules.CashRegister.Application.Contract.DTOs.Transactions;

public sealed record CashRegisterTransactionDto(
    long Id,
    long CashRegisterId,
    long SessionId,
    CashRegisterTransactionType Type,
    decimal Amount,
    DateTimeOffset OccurredAt,
    long CreatedByUserId,
    string? Description,
    string? ReferenceType,
    long? ReferenceId);

public sealed class CashRegisterTransactionListRequestDto : PagedParamData
{
    public long? SalonId { get; set; }
    public long? SessionId { get; set; }
    public CashRegisterTransactionType? Type { get; set; }
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }
}

public sealed record CashMovementRequestDto(
    decimal Amount,
    string IdempotencyKey,
    string? Description);

public sealed record CashRegisterBalanceDto(
    long CashRegisterId,
    long? SessionId,
    decimal ExpectedBalance);

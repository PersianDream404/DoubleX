using Modules.CashRegister.Domain.Entities;

namespace Modules.CashRegister.Application.Contract.DTOs.Sessions;

public sealed record CashRegisterSessionDto(
    long Id,
    long SalonId,
    long CashRegisterId,
    long OpenedByUserId,
    DateTimeOffset OpenedAt,
    decimal OpeningBalance,
    long? ClosedByUserId,
    DateTimeOffset? ClosedAt,
    decimal? ExpectedClosingBalance,
    decimal? ActualClosingBalance,
    decimal? Difference,
    CashRegisterSessionStatus Status);

public sealed record OpenCashRegisterSessionRequestDto(decimal OpeningBalance);

public sealed record CloseCashRegisterSessionRequestDto(decimal ActualClosingBalance);

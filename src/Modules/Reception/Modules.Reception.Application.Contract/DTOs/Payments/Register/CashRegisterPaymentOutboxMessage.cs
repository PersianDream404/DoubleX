namespace Modules.Reception.Application.Contract.DTOs.Payments.Register;

public sealed record CashRegisterPaymentOutboxMessage(
    long Id,
    long SalonId,
    long CashRegisterId,
    long CashRegisterSessionId,
    long ReceptionId,
    long UserId,
    decimal Amount,
    DateTimeOffset OccurredAt,
    string IdempotencyKey,
    string? Description);

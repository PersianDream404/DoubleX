namespace Modules.Reception.Application.Contract.DTOs.Payments.Register;

public sealed record RegisterReceptionPaymentRequestDto(
    long SalonId,
    decimal Amount,
    string PaymentMethod,
    DateTimeOffset PaidAt,
    string? TransactionReference,
    string IdempotencyKey,
    string? Description,
    long? CashRegisterId);

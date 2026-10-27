using SharedKernel.Base;

namespace Modules.Reception.Domain.Entities;

public sealed class CashRegisterPaymentOutbox : BaseEntityIdentity
{
    private CashRegisterPaymentOutbox() { }

    private CashRegisterPaymentOutbox(
        long salonId,
        long cashRegisterId,
        long cashRegisterSessionId,
        long receptionId,
        long userId,
        decimal amount,
        DateTimeOffset occurredAt,
        string idempotencyKey,
        string? description)
    {
        SalonId = salonId;
        CashRegisterId = cashRegisterId;
        CashRegisterSessionId = cashRegisterSessionId;
        ReceptionId = receptionId;
        UserId = userId;
        Amount = amount;
        OccurredAt = occurredAt;
        IdempotencyKey = idempotencyKey;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        NextAttemptAt = DateTimeOffset.UtcNow;
        OutboxStatus = CashRegisterPaymentOutboxStatus.Pending;
    }

    public long SalonId { get; private set; }
    public long CashRegisterId { get; private set; }
    public long CashRegisterSessionId { get; private set; }
    public long ReceptionId { get; private set; }
    public long UserId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string IdempotencyKey { get; private set; } = null!;
    public string? Description { get; private set; }
    public CashRegisterPaymentOutboxStatus OutboxStatus { get; private set; }
    public int Attempts { get; private set; }
    public DateTimeOffset NextAttemptAt { get; private set; }
    public string? LastError { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }

    public static CashRegisterPaymentOutbox Create(
        long salonId,
        long cashRegisterId,
        long cashRegisterSessionId,
        long receptionId,
        long userId,
        decimal amount,
        DateTimeOffset occurredAt,
        string idempotencyKey,
        string? description) =>
        new(
            salonId,
            cashRegisterId,
            cashRegisterSessionId,
            receptionId,
            userId,
            amount,
            occurredAt,
            idempotencyKey,
            description);

    public void MarkProcessed(DateTimeOffset processedAt)
    {
        OutboxStatus = CashRegisterPaymentOutboxStatus.Processed;
        ProcessedAt = processedAt;
        LastError = null;
    }

    public void MarkFailed(string error, DateTimeOffset attemptedAt)
    {
        Attempts++;
        var retryDelay = Math.Min(5 * Math.Pow(2, Math.Min(Attempts - 1, 6)), 300);
        NextAttemptAt = attemptedAt.AddSeconds(retryDelay);
        LastError = error.Length <= 2000 ? error : error[..2000];
    }
}

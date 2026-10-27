using SharedKernel.Base;

namespace Modules.CashRegister.Domain.Entities;

public sealed class CashRegisterTransaction : BaseEntityIdentity
{
    private CashRegisterTransaction() { }

    private CashRegisterTransaction(
        long salonId,
        long cashRegisterId,
        long sessionId,
        CashRegisterTransactionType type,
        decimal amount,
        DateTimeOffset occurredAt,
        long createdByUserId,
        string? description,
        string? referenceType,
        long? referenceId,
        string idempotencyKey)
    {
        SalonId = salonId;
        CashRegisterId = cashRegisterId;
        SessionId = sessionId;
        Type = type;
        Amount = amount;
        OccurredAt = occurredAt;
        CreatedByUserId = createdByUserId;
        Description = Normalize(description);
        ReferenceType = Normalize(referenceType);
        ReferenceId = referenceId;
        IdempotencyKey = idempotencyKey.Trim();
    }

    public long SalonId { get; private set; }
    public long CashRegisterId { get; private set; }
    public long SessionId { get; private set; }
    public CashRegisterTransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public long CreatedByUserId { get; private set; }
    public string? Description { get; private set; }
    public string? ReferenceType { get; private set; }
    public long? ReferenceId { get; private set; }
    public string IdempotencyKey { get; private set; } = null!;
    public CashRegisterSession Session { get; private set; } = null!;

    public static CashRegisterTransaction Create(
        long salonId,
        long cashRegisterId,
        long sessionId,
        CashRegisterTransactionType type,
        decimal amount,
        DateTimeOffset occurredAt,
        long createdByUserId,
        string? description,
        string? referenceType,
        long? referenceId,
        string idempotencyKey) =>
        new(
            salonId,
            cashRegisterId,
            sessionId,
            type,
            amount,
            occurredAt,
            createdByUserId,
            description,
            referenceType,
            referenceId,
            idempotencyKey);

    internal void AttachTo(CashRegisterSession session) => Session = session;

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

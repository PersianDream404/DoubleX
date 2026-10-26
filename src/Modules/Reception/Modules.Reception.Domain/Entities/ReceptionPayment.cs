using SharedKernel.Base;

namespace Modules.Reception.Domain.Entities;

public sealed class ReceptionPayment : BaseEntityIdentity
{
    public long ReceptionId { get; private set; }
    public decimal Amount { get; private set; }
    public string PaymentMethod { get; private set; } = null!;
    public DateTimeOffset PaidAt { get; private set; }
    public ReceptionPaymentStatus PaymentStatus { get; private set; }
    public string? TransactionReference { get; private set; }
    public string IdempotencyKey { get; private set; } = null!;
    public string? Description { get; private set; }
    public Reception Reception { get; private set; } = null!;

    private ReceptionPayment() { }

    private ReceptionPayment(
        long receptionId,
        decimal amount,
        string paymentMethod,
        DateTimeOffset paidAt,
        string? transactionReference,
        string idempotencyKey,
        string? description)
    {
        ReceptionId = receptionId;
        Amount = amount;
        PaymentMethod = paymentMethod.Trim();
        PaidAt = paidAt;
        PaymentStatus = ReceptionPaymentStatus.Succeeded;
        TransactionReference = Normalize(transactionReference);
        IdempotencyKey = idempotencyKey.Trim();
        Description = Normalize(description);
    }

    internal static ReceptionPayment Create(
        long receptionId,
        decimal amount,
        string paymentMethod,
        DateTimeOffset paidAt,
        string? transactionReference,
        string idempotencyKey,
        string? description) =>
        new(receptionId, amount, paymentMethod, paidAt, transactionReference, idempotencyKey, description);

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

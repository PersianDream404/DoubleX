using SharedKernel.Base;

namespace Modules.Finance.Domain.Entities;

public enum PersonnelEarningType
{
    FixedSalary = 1,
    ServiceCommission = 2
}

public enum FinanceRecordStatus
{
    Active = 1,
    Cancelled = 2
}

public sealed class PersonnelEarning : BaseEntityIdentity
{
    public long SalonId { get; private set; }
    public long PersonnelId { get; private set; }
    public long? ReceptionId { get; private set; }
    public long? ReceptionServiceId { get; private set; }
    public PersonnelEarningType EarningType { get; private set; }
    public decimal CalculationBaseAmount { get; private set; }
    public decimal? CommissionRate { get; private set; }
    public decimal Amount { get; private set; }
    public DateOnly? PeriodStart { get; private set; }
    public DateOnly? PeriodEnd { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public DateTimeOffset CalculatedAt { get; private set; }
    public FinanceRecordStatus Status { get; private set; }
    public string? Description { get; private set; }
    public string IdempotencyKey { get; private set; } = null!;

    private PersonnelEarning() { }

    private PersonnelEarning(
        long salonId,
        long personnelId,
        long? receptionId,
        long? receptionServiceId,
        PersonnelEarningType earningType,
        decimal calculationBaseAmount,
        decimal? commissionRate,
        decimal amount,
        DateOnly? periodStart,
        DateOnly? periodEnd,
        DateTimeOffset occurredAt,
        string idempotencyKey,
        string? description)
    {
        SalonId = salonId;
        PersonnelId = personnelId;
        ReceptionId = receptionId;
        ReceptionServiceId = receptionServiceId;
        EarningType = earningType;
        CalculationBaseAmount = calculationBaseAmount;
        CommissionRate = commissionRate;
        Amount = amount;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
        OccurredAt = occurredAt;
        CalculatedAt = DateTimeOffset.UtcNow;
        Status = FinanceRecordStatus.Active;
        IdempotencyKey = idempotencyKey.Trim();
        Description = Normalize(description);
    }

    public static PersonnelEarning Create(
        long salonId,
        long personnelId,
        long? receptionId,
        long? receptionServiceId,
        PersonnelEarningType earningType,
        decimal calculationBaseAmount,
        decimal? commissionRate,
        decimal amount,
        DateOnly? periodStart,
        DateOnly? periodEnd,
        DateTimeOffset occurredAt,
        string idempotencyKey,
        string? description) =>
        new(
            salonId,
            personnelId,
            receptionId,
            receptionServiceId,
            earningType,
            calculationBaseAmount,
            commissionRate,
            amount,
            periodStart,
            periodEnd,
            occurredAt,
            idempotencyKey,
            description);

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class PersonnelSettlement : BaseEntityIdentity
{
    private readonly List<PersonnelSettlementItem> _items = [];

    public long SalonId { get; private set; }
    public long PersonnelId { get; private set; }
    public long CreatedBy { get; private set; }
    public PersonnelEarningType EarningType { get; private set; }
    public decimal Amount { get; private set; }
    public DateTimeOffset PaidAt { get; private set; }
    public string PaymentMethod { get; private set; } = null!;
    public string IdempotencyKey { get; private set; } = null!;
    public FinanceRecordStatus Status { get; private set; }
    public string? Description { get; private set; }
    public IReadOnlyCollection<PersonnelSettlementItem> Items => _items;

    private PersonnelSettlement() { }

    private PersonnelSettlement(
        long salonId,
        long personnelId,
        long createdBy,
        PersonnelEarningType earningType,
        decimal amount,
        DateTimeOffset paidAt,
        string paymentMethod,
        string idempotencyKey,
        string? description)
    {
        SalonId = salonId;
        PersonnelId = personnelId;
        CreatedBy = createdBy;
        EarningType = earningType;
        Amount = amount;
        PaidAt = paidAt;
        PaymentMethod = paymentMethod.Trim();
        IdempotencyKey = idempotencyKey.Trim();
        Status = FinanceRecordStatus.Active;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public static PersonnelSettlement Create(
        long salonId,
        long personnelId,
        long createdBy,
        PersonnelEarningType earningType,
        decimal amount,
        DateTimeOffset paidAt,
        string paymentMethod,
        string idempotencyKey,
        string? description) =>
        new(salonId, personnelId, createdBy, earningType, amount, paidAt, paymentMethod, idempotencyKey, description);

    public void Allocate(long earningId, decimal amount)
    {
        _items.Add(PersonnelSettlementItem.Create(this, earningId, amount));
    }
}

public sealed class PersonnelSettlementItem : BaseEntityIdentity
{
    public long SettlementId { get; private set; }
    public long PersonnelEarningId { get; private set; }
    public decimal Amount { get; private set; }
    public PersonnelSettlement Settlement { get; private set; } = null!;
    public PersonnelEarning Earning { get; private set; } = null!;

    private PersonnelSettlementItem() { }

    private PersonnelSettlementItem(PersonnelSettlement settlement, long personnelEarningId, decimal amount)
    {
        Settlement = settlement;
        SettlementId = settlement.Id;
        PersonnelEarningId = personnelEarningId;
        Amount = amount;
    }

    public static PersonnelSettlementItem Create(
        PersonnelSettlement settlement,
        long earningId,
        decimal amount) =>
        new(settlement, earningId, amount);
}

public sealed class Expense : BaseEntityIdentity
{
    public long SalonId { get; private set; }
    public long CreatedBy { get; private set; }
    public string Category { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string? Description { get; private set; }
    public string IdempotencyKey { get; private set; } = null!;
    public FinanceRecordStatus Status { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }

    private Expense() { }

    private Expense(
        long salonId,
        long createdBy,
        string category,
        decimal amount,
        DateTimeOffset occurredAt,
        string? description,
        string idempotencyKey)
    {
        SalonId = salonId;
        CreatedBy = createdBy;
        Category = category.Trim();
        Amount = amount;
        OccurredAt = occurredAt;
        Description = Normalize(description);
        IdempotencyKey = idempotencyKey.Trim();
        Status = FinanceRecordStatus.Active;
    }

    public static Expense Create(
        long salonId,
        long createdBy,
        string category,
        decimal amount,
        DateTimeOffset occurredAt,
        string? description,
        string idempotencyKey) =>
        new(salonId, createdBy, category, amount, occurredAt, description, idempotencyKey);

    public bool Cancel(DateTimeOffset cancelledAt, string reason)
    {
        if (Status == FinanceRecordStatus.Cancelled || string.IsNullOrWhiteSpace(reason))
        {
            return false;
        }

        Status = FinanceRecordStatus.Cancelled;
        CancelledAt = cancelledAt;
        CancellationReason = reason.Trim();
        return true;
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

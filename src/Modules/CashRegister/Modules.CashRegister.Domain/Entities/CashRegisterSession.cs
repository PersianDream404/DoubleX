using SharedKernel.Base;

namespace Modules.CashRegister.Domain.Entities;

public sealed class CashRegisterSession : BaseEntityIdentity
{
    private readonly List<CashRegisterTransaction> _transactions = [];

    private CashRegisterSession() { }

    private CashRegisterSession(
        long salonId,
        long cashRegisterId,
        long openedByUserId,
        DateTimeOffset openedAt,
        decimal openingBalance)
    {
        SalonId = salonId;
        CashRegisterId = cashRegisterId;
        OpenedByUserId = openedByUserId;
        OpenedAt = openedAt;
        OpeningBalance = openingBalance;
        SessionStatus = CashRegisterSessionStatus.Open;
    }

    public long SalonId { get; private set; }
    public long CashRegisterId { get; private set; }
    public long OpenedByUserId { get; private set; }
    public DateTimeOffset OpenedAt { get; private set; }
    public decimal OpeningBalance { get; private set; }
    public long? ClosedByUserId { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public decimal? ActualClosingBalance { get; private set; }
    public decimal? ExpectedClosingBalance { get; private set; }
    public decimal? Difference { get; private set; }
    public CashRegisterSessionStatus SessionStatus { get; private set; }
    public int Revision { get; private set; }
    public IReadOnlyCollection<CashRegisterTransaction> Transactions => _transactions;

    public static CashRegisterSession Open(
        long salonId,
        long cashRegisterId,
        long openedByUserId,
        DateTimeOffset openedAt,
        decimal openingBalance) =>
        new(salonId, cashRegisterId, openedByUserId, openedAt, openingBalance);

    public bool Close(
        long closedByUserId,
        DateTimeOffset closedAt,
        decimal expectedBalance,
        decimal actualBalance)
    {
        if (SessionStatus != CashRegisterSessionStatus.Open || actualBalance < 0)
        {
            return false;
        }

        ClosedByUserId = closedByUserId;
        ClosedAt = closedAt;
        ExpectedClosingBalance = expectedBalance;
        ActualClosingBalance = actualBalance;
        Difference = actualBalance - expectedBalance;
        SessionStatus = CashRegisterSessionStatus.Closed;
        Revision++;
        return true;
    }

    public bool RegisterTransaction(
        CashRegisterTransactionType type,
        DateTimeOffset occurredAt,
        decimal amount)
    {
        if (SessionStatus == CashRegisterSessionStatus.Open)
        {
            Revision++;
            return true;
        }

        if (type != CashRegisterTransactionType.ReceptionPayment ||
            ClosedAt is null ||
            occurredAt > ClosedAt.Value ||
            ExpectedClosingBalance is null ||
            ActualClosingBalance is null)
            return false;

        ExpectedClosingBalance += amount;
        Difference = ActualClosingBalance - ExpectedClosingBalance;
        Revision++;
        return true;
    }

    public void AddOpeningBalanceTransaction(long userId)
    {
        if (OpeningBalance <= 0)
        {
            return;
        }

        var transaction = CashRegisterTransaction.Create(
            SalonId,
            CashRegisterId,
            Id,
            CashRegisterTransactionType.OpeningBalance,
            OpeningBalance,
            OpenedAt,
            userId,
            "موجودی اولیه هنگام افتتاح صندوق",
            "CashRegisterSession",
            null,
            $"opening-{Guid.NewGuid():N}");
        transaction.AttachTo(this);
        _transactions.Add(transaction);
    }
}

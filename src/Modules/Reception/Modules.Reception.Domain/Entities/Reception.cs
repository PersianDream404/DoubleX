using SharedKernel.Base;

namespace Modules.Reception.Domain.Entities;

public sealed class Reception : BaseEntityIdentity
{
    private const decimal MaximumAmount = 9999999999999999.99m;
    private const decimal MaximumServiceQuantity = 1000000000m;
    private readonly List<ReceptionService> _services = [];
    private readonly List<ReceptionPayment> _payments = [];

    public long SalonId { get; private set; }
    public string ReceptionNumber { get; private set; } = null!;
    public long CustomerId { get; private set; }
    public string CustomerName { get; private set; } = null!;
    public long ReceptionistId { get; private set; }
    public long? ServiceTariffId { get; private set; }
    public long? AppointmentId { get; private set; }
    public DateTimeOffset ReceptionDate { get; private set; }
    public ReceptionStatus ReceptionStatus { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal TotalDiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public ReceptionFinancialStatus ReceptionFinancialStatus { get; private set; }
    public string? Notes { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public int Revision { get; private set; }
    public IReadOnlyCollection<ReceptionService> Services => _services;
    public IReadOnlyCollection<ReceptionPayment> Payments => _payments;

    private Reception() { }

    private Reception(
        long salonId,
        string receptionNumber,
        long customerId,
        string customerName,
        long receptionistId,
        long serviceTariffId,
        long? appointmentId,
        DateTimeOffset receptionDate,
        string? notes)
    {
        SalonId = salonId;
        ReceptionNumber = receptionNumber;
        CustomerId = customerId;
        CustomerName = customerName;
        ReceptionistId = receptionistId;
        ServiceTariffId = serviceTariffId;
        AppointmentId = appointmentId;
        ReceptionDate = receptionDate;
        Notes = Normalize(notes);
        ReceptionStatus = ReceptionStatus.Open;
        ReceptionFinancialStatus = ReceptionFinancialStatus.Unpaid;
    }

    public static Reception Create(
        long salonId,
        long customerId,
        string customerName,
        long receptionistId,
        long serviceTariffId,
        DateTimeOffset receptionDate,
        string? notes,
        long? appointmentId = null) =>
        new(
            salonId,
            $"R-{Guid.NewGuid():N}",
            customerId,
            customerName,
            receptionistId,
            serviceTariffId,
            appointmentId,
            receptionDate,
            notes);

    public bool TryAddService(
        long serviceId,
        long tariffId,
        string serviceName,
        long? personnelId,
        decimal quantity,
        decimal unitPrice,
        decimal discountAmount)
    {
        if (ReceptionStatus is ReceptionStatus.Completed or ReceptionStatus.Cancelled ||
            quantity <= 0 || quantity > MaximumServiceQuantity ||
            unitPrice < 0 || unitPrice > MaximumAmount || discountAmount < 0 ||
            discountAmount > quantity * unitPrice)
        {
            return false;
        }

        var nextSubtotal = SubTotal + (quantity * unitPrice);
        var nextDiscount = TotalDiscountAmount + discountAmount;
        if (nextSubtotal > MaximumAmount || nextDiscount > MaximumAmount ||
            nextSubtotal - nextDiscount > MaximumAmount)
        {
            return false;
        }

        _services.Add(ReceptionService.Create(
            Id,
            serviceId,
            tariffId,
            serviceName,
            personnelId,
            quantity,
            unitPrice,
            discountAmount));
        RecalculateTotals();
        Revision++;
        return true;
    }

    public bool TryAddPayment(
        decimal amount,
        string paymentMethod,
        DateTimeOffset paidAt,
        string? transactionReference,
        string idempotencyKey,
        string? description)
    {
        if (ReceptionStatus == ReceptionStatus.Cancelled ||
            amount <= 0 ||
            amount > MaximumAmount ||
            _payments.Any(payment => payment.IdempotencyKey == idempotencyKey) ||
            amount > RemainingAmount)
        {
            return false;
        }

        _payments.Add(ReceptionPayment.Create(
            Id,
            amount,
            paymentMethod,
            paidAt,
            transactionReference,
            idempotencyKey,
            description));
        RecalculateFinancialStatus();
        Revision++;
        return true;
    }

    public bool ChangeServiceStatus(long serviceId, ReceptionServiceStatus status)
    {
        var service = _services.SingleOrDefault(item => item.Id == serviceId);
        if (service is null)
        {
            return false;
        }

        if (status == ReceptionServiceStatus.Cancelled &&
            TotalAmount - service.FinalAmount < NetPaidAmount)
        {
            return false;
        }

        if (!service.ChangeStatus(status))
        {
            return false;
        }

        RecalculateTotals();
        Revision++;
        return true;
    }

    public bool MarkMaterialMovementRegistered(long receptionServiceId, string idempotencyKey)
    {
        var service = _services.SingleOrDefault(item => item.Id == receptionServiceId);
        if (service is null || !service.MarkMaterialMovementRegistered(idempotencyKey))
        {
            return false;
        }

        Revision++;
        return true;
    }

    public bool MarkMaterialMovementReversed(long receptionServiceId, long materialRecordId)
    {
        var service = _services.SingleOrDefault(item => item.Id == receptionServiceId);
        if (service is null || !service.MarkMaterialMovementReversed(materialRecordId))
        {
            return false;
        }

        Revision++;
        return true;
    }

    public decimal NetPaidAmount =>
        _payments.Where(payment => payment.PaymentStatus == ReceptionPaymentStatus.Succeeded)
            .Sum(payment => payment.Amount);

    public decimal RemainingAmount => TotalAmount - NetPaidAmount;

    public bool ChangeStatus(ReceptionStatus status)
    {
        if (ReceptionStatus is ReceptionStatus.Completed or ReceptionStatus.Cancelled ||
            status is not (ReceptionStatus.InProgress or ReceptionStatus.Completed or ReceptionStatus.Cancelled))
        {
            return false;
        }

        if (status == ReceptionStatus.Cancelled &&
            (_services.SelectMany(service => service.Materials)
                .Any(material => !material.MovementRegistered || !material.MovementReversed) ||
             _payments.Any(payment => payment.PaymentStatus == ReceptionPaymentStatus.Succeeded)))
        {
            return false;
        }

        ReceptionStatus = status;
        Revision++;
        return true;
    }

    public void RecalculateTotals()
    {
        var billableServices = _services.Where(service =>
            service.ServiceStatus != ReceptionServiceStatus.Cancelled);
        SubTotal = billableServices.Sum(service => service.Quantity * service.UnitPrice);
        TotalDiscountAmount = billableServices.Sum(service => service.DiscountAmount);
        TotalAmount = SubTotal - TotalDiscountAmount;
        RecalculateFinancialStatus();
    }

    private void RecalculateFinancialStatus() =>
        ReceptionFinancialStatus = NetPaidAmount <= 0 ? ReceptionFinancialStatus.Unpaid :
            NetPaidAmount < TotalAmount
                ? ReceptionFinancialStatus.PartiallyPaid
                : ReceptionFinancialStatus.Paid;

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

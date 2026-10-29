using SharedKernel.Base;

namespace Modules.Appointment.Domain.Entities;

public sealed class Appointment : BaseEntityIdentity
{
    private const decimal MaximumAmount = 9999999999999999.99m;

    private Appointment() { }

    public long SalonId { get; private set; }
    public long CustomerId { get; private set; }
    public string CustomerName { get; private set; } = null!;
    public long ServiceId { get; private set; }
    public string ServiceName { get; private set; } = null!;
    public long ServiceTariffId { get; private set; }
    public long PersonnelId { get; private set; }
    public string PersonnelName { get; private set; } = null!;
    public DateTimeOffset StartsAt { get; private set; }
    public DateTimeOffset EndsAt { get; private set; }
    public int DurationMinutes { get; private set; }
    public string? Notes { get; private set; }
    public decimal BaseAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal FinalAmount { get; private set; }
    public AppointmentNotificationChannel NotificationChannel { get; private set; }
    public AppointmentStatus AppointmentStatus { get; private set; }
    public Guid IdempotencyKey { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public static Appointment Create(
        long salonId,
        long customerId,
        string customerName,
        long serviceId,
        string serviceName,
        long serviceTariffId,
        long personnelId,
        string personnelName,
        DateTimeOffset startsAt,
        int durationMinutes,
        string? notes,
        decimal baseAmount,
        decimal discountAmount,
        AppointmentNotificationChannel notificationChannel,
        Guid idempotencyKey,
        DateTimeOffset createdAt)
    {
        var utcStartsAt = startsAt.ToUniversalTime();
        return new Appointment
        {
            SalonId = salonId,
            CustomerId = customerId,
            CustomerName = customerName.Trim(),
            ServiceId = serviceId,
            ServiceName = serviceName.Trim(),
            ServiceTariffId = serviceTariffId,
            PersonnelId = personnelId,
            PersonnelName = personnelName.Trim(),
            StartsAt = utcStartsAt,
            EndsAt = utcStartsAt.AddMinutes(durationMinutes),
            DurationMinutes = durationMinutes,
            Notes = Normalize(notes),
            BaseAmount = baseAmount,
            DiscountAmount = discountAmount,
            FinalAmount = baseAmount - discountAmount,
            NotificationChannel = notificationChannel,
            AppointmentStatus = AppointmentStatus.Pending,
            IdempotencyKey = idempotencyKey,
            CreatedAt = createdAt.ToUniversalTime(),
            UpdatedAt = createdAt.ToUniversalTime()
        };
    }

    public bool MatchesCreateRequest(
        long customerId,
        long serviceId,
        long serviceTariffId,
        long personnelId,
        DateTimeOffset startsAt,
        decimal discountAmount,
        string? notes,
        AppointmentNotificationChannel notificationChannel) =>
        CustomerId == customerId &&
        ServiceId == serviceId &&
        ServiceTariffId == serviceTariffId &&
        PersonnelId == personnelId &&
        StartsAt == startsAt.ToUniversalTime() &&
        DiscountAmount == discountAmount &&
        Notes == Normalize(notes) &&
        NotificationChannel == notificationChannel;

    public bool UpdateSchedule(
        DateTimeOffset startsAt,
        long personnelId,
        string personnelName,
        string? notes,
        DateTimeOffset updatedAt)
    {
        if (AppointmentStatus is not (AppointmentStatus.Pending or AppointmentStatus.Confirmed))
            return false;

        StartsAt = startsAt.ToUniversalTime();
        EndsAt = StartsAt.AddMinutes(DurationMinutes);
        PersonnelId = personnelId;
        PersonnelName = personnelName.Trim();
        Notes = Normalize(notes);
        UpdatedAt = updatedAt.ToUniversalTime();
        return true;
    }

    public bool ChangeStatus(AppointmentStatus status, DateTimeOffset updatedAt)
    {
        var allowed = AppointmentStatus switch
        {
            AppointmentStatus.Pending =>
                status is AppointmentStatus.Confirmed or AppointmentStatus.Cancelled,
            AppointmentStatus.Confirmed =>
                status is AppointmentStatus.Cancelled or AppointmentStatus.NoShow,
            _ => false
        };
        if (!allowed ||
            (status == AppointmentStatus.NoShow && updatedAt < StartsAt))
            return false;

        AppointmentStatus = status;
        UpdatedAt = updatedAt.ToUniversalTime();
        return true;
    }

    public bool IsValidForReception(long salonId, long customerId) =>
        SalonId == salonId &&
        CustomerId == customerId &&
        AppointmentStatus == AppointmentStatus.Confirmed;

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

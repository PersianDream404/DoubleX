using SharedKernel.Base;

namespace Modules.Reception.Domain.Entities;

public enum PersonnelCommissionOutboxStatus
{
    Pending = 1,
    Processed = 2
}

public sealed class PersonnelCommissionOutbox : BaseEntityIdentity
{
    private PersonnelCommissionOutbox() { }

    private PersonnelCommissionOutbox(long salonId, long receptionId, long receptionServiceId)
    {
        EventId = Guid.NewGuid();
        SalonId = salonId;
        ReceptionId = receptionId;
        ReceptionServiceId = receptionServiceId;
        OutboxStatus = PersonnelCommissionOutboxStatus.Pending;
        NextAttemptAt = DateTimeOffset.UtcNow;
    }

    public Guid EventId { get; private set; }
    public long SalonId { get; private set; }
    public long ReceptionId { get; private set; }
    public long ReceptionServiceId { get; private set; }
    public PersonnelCommissionOutboxStatus OutboxStatus { get; private set; }
    public int Attempts { get; private set; }
    public DateTimeOffset NextAttemptAt { get; private set; }
    public string? LastError { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }

    public static PersonnelCommissionOutbox Create(
        long salonId,
        long receptionId,
        long receptionServiceId) =>
        new(salonId, receptionId, receptionServiceId);

    public void MarkProcessed(DateTimeOffset processedAt)
    {
        OutboxStatus = PersonnelCommissionOutboxStatus.Processed;
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

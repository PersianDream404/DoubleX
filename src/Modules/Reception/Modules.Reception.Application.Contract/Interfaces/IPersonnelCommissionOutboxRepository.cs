using Modules.Reception.Application.Contract.DTOs;

namespace Modules.Reception.Application.Contract.Interfaces;

public interface IPersonnelCommissionOutboxRepository
{
    Task<IReadOnlyList<PersonnelCommissionOutboxMessage>> GetPendingAsync(
        int batchSize,
        DateTimeOffset now,
        CancellationToken cancellationToken);

    Task MarkProcessedAsync(
        long id,
        DateTimeOffset processedAt,
        CancellationToken cancellationToken);

    Task MarkFailedAsync(
        long id,
        string error,
        DateTimeOffset attemptedAt,
        CancellationToken cancellationToken);
}

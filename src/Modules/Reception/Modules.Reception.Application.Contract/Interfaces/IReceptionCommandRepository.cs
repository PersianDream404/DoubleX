using ReceptionEntity = Modules.Reception.Domain.Entities.Reception;

namespace Modules.Reception.Application.Contract.Interfaces;

public interface IReceptionCommandRepository
{
    Task<ReceptionEntity?> GetTrackedAsync(long id, long salonId, CancellationToken cancellationToken);
    Task AddAsync(ReceptionEntity reception, CancellationToken cancellationToken);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
}

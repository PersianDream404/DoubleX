using ReceptionEntity = Modules.Reception.Domain.Entities.Reception;
using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Application.Contract.Interfaces;

public interface IReceptionCommandRepository
{
    Task<ReceptionEntity?> GetTrackedAsync(long id, long salonId, CancellationToken cancellationToken);
    void AddCashRegisterPaymentOutbox(CashRegisterPaymentOutbox outbox);
    Task AddAsync(ReceptionEntity reception, CancellationToken cancellationToken);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
}

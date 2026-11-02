using ReceptionEntity = Modules.Reception.Domain.Entities.Reception;
using Modules.Reception.Domain.Entities;
using Ardalis.Result;

namespace Modules.Reception.Application.Contract.Interfaces;

public interface IReceptionCommandRepository
{
    Task<ReceptionEntity?> GetTrackedAsync(long id, long salonId, CancellationToken cancellationToken);
    void AddCashRegisterPaymentOutbox(CashRegisterPaymentOutbox outbox);
    void AddPersonnelCommissionOutbox(PersonnelCommissionOutbox outbox);
    Task<bool> AppointmentAlreadyLinkedAsync(long appointmentId, CancellationToken cancellationToken);
    Task<Result<T>> AddAsync<T>(
        ReceptionEntity reception,
        Func<ReceptionEntity, CancellationToken, Task<Result<T>>> transactionWork,
        CancellationToken cancellationToken);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
}

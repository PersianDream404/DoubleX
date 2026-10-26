using Framwork.PagedList;
using Modules.Reception.Domain.ReadModels;

namespace Modules.Reception.Domain.Interfaces;

public interface IReceptionQueryRepository
{
    Task<PagedList<ReceptionListReadModel>> GetAllAsync(
        long salonId,
        long? customerId,
        bool unpaidOnly,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<ReceptionDetailsReadModel?> GetByIdAsync(
        long id,
        long salonId,
        CancellationToken cancellationToken);
}

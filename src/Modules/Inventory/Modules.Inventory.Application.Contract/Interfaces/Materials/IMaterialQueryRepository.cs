using Framwork.PagedList;
using Modules.Inventory.Application.Contract.DTOs.Materials;
using Modules.Inventory.Domain.Entities;

namespace Modules.Inventory.Application.Contract.Interfaces.Materials;

public interface IMaterialQueryRepository
{
    Task<PagedList<MaterialDto>> GetAllAsync(
        GetMaterialsRequestDto request,
        CancellationToken cancellationToken);
    Task<MaterialDto?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<IReadOnlyList<MaterialReceptionLookupDto>> GetReceptionLookupsAsync(
        IReadOnlyCollection<long> materialIds,
        CancellationToken cancellationToken);
    Task<Material?> GetEntityByIdAsync(long id, CancellationToken cancellationToken);
}

using Framwork.PagedList;
using Modules.Inventory.Application.Contract.DTOs.Units;
using Modules.Inventory.Domain.Entities;

namespace Modules.Inventory.Application.Contract.Interfaces.Units;

public interface IUnitQueryRepository
{
    Task<PagedList<UnitDto>> GetAllAsync(
        GetUnitsRequestDto request,
        CancellationToken cancellationToken);
    Task<UnitDto?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<Unit?> GetEntityByIdAsync(long id, CancellationToken cancellationToken);
}

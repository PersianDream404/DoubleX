using Framwork.PagedList;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Modules.Inventory.Application.Contract.DTOs.Units;
using Modules.Inventory.Application.Contract.Interfaces.Units;
using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Persistence.Context;
using Modules.Inventory.Persistence.Mapper.Units;

namespace Modules.Inventory.Persistence.Repositories.Units;

public sealed class UnitQueryRepository(InventoryReadDbContext context)
    : IUnitQueryRepository
{
    public async Task<PagedList<UnitDto>> GetAllAsync(
        GetUnitsRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = context.Units.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            var term = request.Q.Trim();
            query = query.Where(unit => unit.Name.Contains(term) || unit.Symbol.Contains(term));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(unit => unit.Status == request.IsActive.Value);
        }

        return await query
            .OrderBy(unit => unit.Name)
            .Select(UnitMapper.ToGetAllDto())
            .ToPagedList(request.PageNumber, request.PageSize, cancellationToken);
    }

    public Task<UnitDto?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        context.Units
            .AsNoTracking()
            .Where(unit => unit.Id == id)
            .Select(UnitMapper.ToGetByIdDto())
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Unit?> GetEntityByIdAsync(long id, CancellationToken cancellationToken) =>
        context.Units.FirstOrDefaultAsync(unit => unit.Id == id, cancellationToken);
}

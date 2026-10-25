using Framwork.PagedList;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Modules.Inventory.Application.Contract.DTOs.Materials;
using Modules.Inventory.Application.Contract.Interfaces.Materials;
using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Persistence.Context;
using Modules.Inventory.Persistence.Mapper.Materials;

namespace Modules.Inventory.Persistence.Repositories.Materials;

public sealed class MaterialQueryRepository(InventoryReadDbContext context)
    : IMaterialQueryRepository
{
    public async Task<PagedList<MaterialDto>> GetAllAsync(
        GetMaterialsRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = context.Materials.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            var term = request.Q.Trim();
            query = query.Where(material =>
                material.Name.Contains(term) ||
                material.Code.Contains(term) ||
                (material.Brand != null && material.Brand.Contains(term)));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(material => material.Status == request.IsActive.Value);
        }

        return await query
            .OrderBy(material => material.Name)
            .Select(MaterialMapper.ToGetAllDto())
            .ToPagedList(request.PageNumber, request.PageSize, cancellationToken);
    }

    public Task<MaterialDto?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        context.Materials
            .AsNoTracking()
            .Where(material => material.Id == id)
            .Select(MaterialMapper.ToGetByIdDto())
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Material?> GetEntityByIdAsync(long id, CancellationToken cancellationToken) =>
        context.Materials.FirstOrDefaultAsync(material => material.Id == id, cancellationToken);
}

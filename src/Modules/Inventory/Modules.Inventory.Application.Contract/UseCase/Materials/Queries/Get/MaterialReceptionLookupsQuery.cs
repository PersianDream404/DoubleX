using Framwork.Bus.Query;
using Modules.Inventory.Application.Contract.DTOs.Materials;

namespace Modules.Inventory.Application.Contract.UseCase;

public sealed record GetMaterialReceptionLookupsQuery(IReadOnlyCollection<long> MaterialIds)
    : IQuery<IReadOnlyList<MaterialReceptionLookupDto>>;

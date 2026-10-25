using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Inventory.Application.Contract.DTOs.Materials;

namespace Modules.Inventory.Application.Contract.UseCase;

public sealed record GetMaterialsQuery(GetMaterialsRequestDto Request) : IQuery<PagedList<MaterialDto>>;

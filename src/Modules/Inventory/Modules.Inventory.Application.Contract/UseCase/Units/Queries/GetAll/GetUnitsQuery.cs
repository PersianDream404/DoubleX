using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Inventory.Application.Contract.DTOs.Units;

namespace Modules.Inventory.Application.Contract.UseCase;

public sealed record GetUnitsQuery(GetUnitsRequestDto Request) : IQuery<PagedList<UnitDto>>;

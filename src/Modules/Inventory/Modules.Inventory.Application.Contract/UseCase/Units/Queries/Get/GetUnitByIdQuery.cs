using Framwork.Bus.Query;
using Modules.Inventory.Application.Contract.DTOs.Units;

namespace Modules.Inventory.Application.Contract.UseCase;

public sealed record GetUnitByIdQuery(long Id) : IQuery<UnitDto>;

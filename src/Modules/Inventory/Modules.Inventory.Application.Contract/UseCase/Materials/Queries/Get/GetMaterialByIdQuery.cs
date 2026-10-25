using Framwork.Bus.Query;
using Modules.Inventory.Application.Contract.DTOs.Materials;

namespace Modules.Inventory.Application.Contract.UseCase;

public sealed record GetMaterialByIdQuery(long Id) : IQuery<MaterialDto>;

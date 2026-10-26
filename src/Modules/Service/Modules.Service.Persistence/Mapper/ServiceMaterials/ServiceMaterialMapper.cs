using System.Linq.Expressions;
using Modules.Service.Domain.Entities;
using Modules.Service.Domain.ReadModels;
using Modules.Service.Persistence.Context;

namespace Modules.Service.Persistence.Mapper.ServiceMaterials;

internal sealed class ServiceMaterialProjection
{
    public required ServiceMaterial Entry { get; init; }
    public required ServiceInventoryMaterialReference Material { get; init; }
    public required ServiceInventoryUnitReference Unit { get; init; }
}

internal static class ServiceMaterialMapper
{
    public static Expression<Func<ServiceMaterialProjection, ServiceMaterialReadModel>> ToReadModel() =>
        projection => new ServiceMaterialReadModel(
            projection.Entry.Id,
            projection.Entry.SalonId,
            projection.Entry.ServiceId,
            projection.Entry.MaterialId,
            projection.Material.Name,
            projection.Unit.Name,
            projection.Unit.Symbol,
            projection.Entry.QuantityPerService,
            projection.Entry.Description);
}

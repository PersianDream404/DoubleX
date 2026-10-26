using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Inventory.Persistence.Context;
using Modules.Personnel.Persistence.Context;
using Modules.Service.Persistence.Context;

namespace Modules.Service.Persistence.Extensions;

public static class ServiceMigrationExtensions
{
    public static async Task ApplyServiceMigrationsAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var salonContext = scope.ServiceProvider.GetRequiredService<PersonnelWriteDbContext>();
        await salonContext.Database.MigrateAsync(cancellationToken);
        var inventoryContext = scope.ServiceProvider.GetRequiredService<InventoryWriteDbContext>();
        await inventoryContext.Database.MigrateAsync(cancellationToken);
        var context = scope.ServiceProvider.GetRequiredService<ServiceWriteDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }
}

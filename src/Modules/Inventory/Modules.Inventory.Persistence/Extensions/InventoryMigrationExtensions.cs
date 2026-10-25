using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Inventory.Persistence.Context;

namespace Modules.Inventory.Persistence.Extensions;

public static class InventoryMigrationExtensions
{
    public static async Task ApplyInventoryMigrationsAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<InventoryWriteDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }
}

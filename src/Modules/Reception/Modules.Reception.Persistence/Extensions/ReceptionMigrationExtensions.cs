using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Reception.Persistence.Context;

namespace Modules.Reception.Persistence.Extensions;

public static class ReceptionMigrationExtensions
{
    public static async Task ApplyReceptionMigrationsAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ReceptionWriteDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }
}

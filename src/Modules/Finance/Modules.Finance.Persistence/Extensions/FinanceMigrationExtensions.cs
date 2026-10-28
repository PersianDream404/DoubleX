using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Finance.Persistence.Context;

namespace Modules.Finance.Persistence.Extensions;

public static class FinanceMigrationExtensions
{
    public static async Task ApplyFinanceMigrationsAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<FinanceWriteDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }
}

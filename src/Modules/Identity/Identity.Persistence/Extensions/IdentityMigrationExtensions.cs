using Identity.Persistence.Context;
using Identity.Domain.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Modules.Identity.Persistence.Extensions;

public static class IdentityMigrationExtensions
{
    public static async Task ApplyIdentityMigrationsAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityWriteDbContext>();
        await context.Database.MigrateAsync(cancellationToken);

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();
        foreach (var role in Roles.All)
        {
            if (await roleManager.RoleExistsAsync(role))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new IdentityRole<long>(role));
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Could not create the fixed role '{role}': {errors}");
            }
        }

        await SuperAdminSeeder.SeedAsync(scope.ServiceProvider, cancellationToken);
    }
}

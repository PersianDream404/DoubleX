using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Appointment.Persistence.Context;

namespace Modules.Appointment.Persistence.Extensions;

public static class AppointmentMigrationExtensions
{
    public static async Task ApplyAppointmentMigrationsAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppointmentWriteDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }
}

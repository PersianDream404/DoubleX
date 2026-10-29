using Microsoft.Extensions.DependencyInjection;
using Modules.Appointment.Application.UseCase.Appointments.Commands.Create;

namespace Modules.Appointment.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAppointmentApplicationServices(
        this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<CreateAppointmentCommandHandler>()
            .AddClasses(classes => classes.Where(type =>
                type.Name.EndsWith("Handler", StringComparison.Ordinal) ||
                type.Name.EndsWith("Validator", StringComparison.Ordinal)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        return services;
    }
}

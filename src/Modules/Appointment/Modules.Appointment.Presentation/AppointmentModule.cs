using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Appointment.Application;
using Modules.Appointment.Persistence;
using Modules.Appointment.Persistence.Extensions;
using Modules.Appointment.Presentation.Security;
using SharedKernel.Interface;

namespace Modules.Appointment.Presentation;

public sealed class AppointmentModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAppointmentInfrastructure(configuration);
        services.AddAppointmentApplicationServices();
        services.AddSingleton<IModulePermissionProvider, AppointmentPermissionProvider>();
        services.Scan(scan => scan
            .FromAssemblyOf<AppointmentModule>()
            .AddClasses(classes => classes.AssignableTo<IEndpoint>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }

    public void RegisterBuilder(IApplicationBuilder builder) =>
        builder.ApplicationServices.ApplyAppointmentMigrationsAsync()
            .GetAwaiter()
            .GetResult();
}

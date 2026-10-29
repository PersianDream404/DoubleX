using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Appointment.Application.Contract.Interfaces;
using Modules.Appointment.Domain.Interfaces;
using Modules.Appointment.Persistence.Context;
using Modules.Appointment.Persistence.Repositories;
using SharedKernel.Constants;

namespace Modules.Appointment.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddAppointmentInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(AppSetting.ConnectionString);
        services.AddDbContext<AppointmentWriteDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddDbContext<AppointmentReadDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped<IAppointmentCommandRepository, AppointmentCommandRepository>();
        services.AddScoped<IAppointmentQueryRepository, AppointmentQueryRepository>();
        return services;
    }
}

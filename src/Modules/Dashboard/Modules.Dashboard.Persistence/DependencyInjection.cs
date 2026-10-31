using Microsoft.Extensions.DependencyInjection;
using Modules.Dashboard.Application.Contract.Interfaces;
using Modules.Dashboard.Persistence.Repositories;

namespace Modules.Dashboard.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddDashboardInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<IDashboardQueryRepository, DashboardQueryRepository>();
        return services;
    }
}

using FluentValidation;
using Framwork.Bus.Query;
using Microsoft.Extensions.DependencyInjection;

namespace Modules.Dashboard.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddDashboardApplicationServices(
        this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        return services;
    }
}

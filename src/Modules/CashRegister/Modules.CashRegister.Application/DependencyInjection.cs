using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Microsoft.Extensions.DependencyInjection;

namespace Modules.CashRegister.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCashRegisterApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        return services;
    }
}

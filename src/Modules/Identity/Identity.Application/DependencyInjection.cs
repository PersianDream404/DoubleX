using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Framwork.Decorator.Command;
using Framwork.Decorator.Query;
using Identity.Application.Common.Mapping;
using Identity.Application.Contract.Services;
using Identity.Application.Users.Services;
using Identity.Domain.Entities;

using Mapster;
using MapsterMapper;

using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Interface;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        //services.AddValidatorsFromAssembly(assembly);


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
        .WithScopedLifetime()
                    );

        services.AddScoped(typeof(IQueryBehavior<,>), typeof(LoggingQueryBehavior<,>));
        services.AddScoped(typeof(IQueryBehavior<,>), typeof(ValidationQueryBehavior<,>));

        services.AddScoped(typeof(ICommandBehavior<,>), typeof(LoggingCommandBehavior<,>));
        services.AddScoped(typeof(ICommandBehavior<,>), typeof(ValidationCommandBehavior<,>));

        services.AddScoped<ICommandBus, CommandBus>();
        services.AddScoped<IQueryBus, QueryBus>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();


        var config = TypeAdapterConfig.GlobalSettings;
        MappingConfig.RegisterMappings();

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}

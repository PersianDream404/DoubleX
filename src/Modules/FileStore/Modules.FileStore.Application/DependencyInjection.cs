using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Framwork.Decorator.Command;
using Framwork.Decorator.Query;

using Mapster;
using MapsterMapper;

using Microsoft.Extensions.DependencyInjection;
using Modules.FileStore.Application.Common.Mapping;
using Modules.FileStore.Application.Contract.Interface.Services;
using Modules.FileStore.Application.Services;

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
        services.AddScoped<IFileStorageService, LocalFileStorageService>();


        #region Mapping
        var config = TypeAdapterConfig.GlobalSettings;
        MappingConfig.RegisterMappings();

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        //// Register All Profile
        //TypeAdapterConfig.GlobalSettings.Scan(typeof(MappingConfig).Assembly);

        //// Add MapsterMapper To  DI
        //services.AddSingleton(TypeAdapterConfig.GlobalSettings);
        //services.AddScoped<IMapper, ServiceMapper>();
        #endregion


        return services;
    }
}

using FluentValidation;
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
        services.AddValidatorsFromAssembly(
             typeof(DependencyInjection).Assembly);

       services.AddScoped<IAuthenticationService, AuthenticationService>();


        var config = TypeAdapterConfig.GlobalSettings;
        MappingConfig.RegisterMappings();

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Customer.Application;
using Modules.Customer.Persistence;
using Modules.Customer.Persistence.Extensions;
using Modules.Customer.Presentation.Endpoints.Customers;
using SharedKernel.Interface;

namespace Modules.Customer.Presentation;

public sealed class CustomerModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomerInfrastructure(configuration);
        services.AddCustomerApplicationServices();
        services.AddControllers()
            .AddApplicationPart(typeof(CustomerModule).Assembly)
            .AddControllersAsServices();

        services.Scan(scan => scan
            .FromAssemblyOf<CreateCustomerEndpoint.EndPoint>()
            .AddClasses(classes => classes.AssignableTo<IEndpoint>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }

    public void RegisterBuilder(IApplicationBuilder builder) =>
        builder.ApplicationServices.ApplyCustomerMigrationsAsync()
            .GetAwaiter()
            .GetResult();
}

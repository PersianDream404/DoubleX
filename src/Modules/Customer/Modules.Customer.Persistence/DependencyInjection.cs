using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Customer.Application.Contract.Interfaces.Customers;
using Modules.Customer.Domain.Interface;
using Modules.Customer.Persistence.Context;
using Modules.Customer.Persistence.Repositories.Customers;
using SharedKernel.Constants;

namespace Modules.Customer.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomerInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(AppSetting.ConnectionString);
        services.AddDbContext<CustomerWriteDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddDbContext<CustomerReadDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ICustomerCommandRepository, CustomerCommandRepository>();
        services.AddScoped<ICustomerQueryRepository, CustomerQueryRepository>();
        return services;
    }
}

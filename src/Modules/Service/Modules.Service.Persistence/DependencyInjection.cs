using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Service.Application.Contract.Interfaces.ServiceCategories;
using Modules.Service.Application.Contract.Interfaces.ServiceMaterials;
using Modules.Service.Application.Contract.Interfaces.ServicePrices;
using Modules.Service.Application.Contract.Interfaces.Services;
using Modules.Service.Persistence.Context;
using Modules.Service.Persistence.Repositories.ServiceCategories;
using Modules.Service.Persistence.Repositories.ServiceMaterials;
using Modules.Service.Persistence.Repositories.ServicePrices;
using Modules.Service.Persistence.Repositories.Services;
using SharedKernel.Constants;

namespace Modules.Service.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(AppSetting.ConnectionString);
        services.AddDbContext<ServiceWriteDbContext>(options => options.UseSqlServer(connectionString));
        services.AddDbContext<ServiceReadDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IServiceCategoryRepository, ServiceCategoryRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IServicePriceRepository, ServicePriceRepository>();
        services.AddScoped<IServiceMaterialRepository, ServiceMaterialRepository>();
        return services;
    }
}

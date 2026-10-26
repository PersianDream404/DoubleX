using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Service.Application.Contract.Interfaces.ServiceCategories;
using Modules.Service.Application.Contract.Interfaces.ServiceMaterials;
using Modules.Service.Application.Contract.Interfaces.ServiceTariffs;
using Modules.Service.Application.Contract.Interfaces.Services;
using Modules.Service.Domain.Interfaces;
using Modules.Service.Persistence.Context;
using Modules.Service.Persistence.Repositories.ServiceCategories;
using Modules.Service.Persistence.Repositories.ServiceMaterials;
using Modules.Service.Persistence.Repositories.ServiceTariffs;
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
        services.AddScoped<IServiceCategoryCommandRepository, ServiceCategoryCommandRepository>();
        services.AddScoped<IServiceCategoryQueryRepository, ServiceCategoryQueryRepository>();
        services.AddScoped<IServiceCommandRepository, ServiceCommandRepository>();
        services.AddScoped<IServiceQueryRepository, ServiceQueryRepository>();
        services.AddScoped<IServiceTariffCommandRepository, ServiceTariffCommandRepository>();
        services.AddScoped<IServiceTariffQueryRepository, ServiceTariffQueryRepository>();
        services.AddScoped<IServiceMaterialCommandRepository, ServiceMaterialCommandRepository>();
        services.AddScoped<IServiceMaterialQueryRepository, ServiceMaterialQueryRepository>();
        return services;
    }
}

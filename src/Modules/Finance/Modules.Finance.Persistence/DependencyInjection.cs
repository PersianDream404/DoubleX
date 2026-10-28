using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Finance.Application.Contract.Interfaces;
using Modules.Finance.Domain.Interfaces;
using Modules.Finance.Persistence.Context;
using Modules.Finance.Persistence.Repositories;
using SharedKernel.Constants;

namespace Modules.Finance.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddFinanceInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(AppSetting.ConnectionString);
        services.AddDbContext<FinanceWriteDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddDbContext<FinanceReadDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped<IFinanceCommandRepository, FinanceCommandRepository>();
        services.AddScoped<IFinanceQueryRepository, FinanceQueryRepository>();
        return services;
    }
}

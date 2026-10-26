using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Reception.Application.Contract.Interfaces;
using Modules.Reception.Domain.Interfaces;
using Modules.Reception.Persistence.Context;
using Modules.Reception.Persistence.Repositories;
using SharedKernel.Constants;

namespace Modules.Reception.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddReceptionInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(AppSetting.ConnectionString);
        services.AddDbContext<ReceptionWriteDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddDbContext<ReceptionReadDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped<IReceptionCommandRepository, ReceptionCommandRepository>();
        services.AddScoped<IReceptionQueryRepository, ReceptionQueryRepository>();
        return services;
    }
}

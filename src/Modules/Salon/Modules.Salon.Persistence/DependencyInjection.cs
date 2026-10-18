using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Personnel.Application.Contract.Interfaces;
using Modules.Personnel.Domain.Interface;
using Modules.Personnel.Persistence.Context;
using Modules.Personnel.Persistence.Repositories;
using SharedKernel.Constants;

namespace Modules.Personnel.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddSalonInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<PersonnelWriteDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString(AppSetting.ConnectionString)));

        services.AddDbContext<PersonnelReadDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString(AppSetting.ConnectionString)));

        services.AddScoped<IPersonnelCommandRepository, PersonnelCommandRepository>();
        services.AddScoped<IPersonnelQueryRepository, PersonnelQueryRepository>();

        return services;
    }
}

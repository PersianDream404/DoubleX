using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Banner.Domain.Interface;
using Modules.Banner.Persistence.Context;
using Modules.Banner.Persistence.Repositories.Banners;
using Modules.Product.Application.Contract.Interface.Banners;
using SharedKernel.Constants;

namespace Modules.Banner.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddBannerInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
          .AddPersistence(configuration)
        ;

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        #region AppSetting Bind

        //services.Configure<FarazSmsOptions>(options =>
        //    configuration.GetSection("FarazSms").Bind(options));

        //services.Configure<S3Configuration>(options =>
        //    configuration.GetSection("S3").Bind(options));
        #endregion


        services.AddDbContext<BannerWriteDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString(AppSetting.ConnectionString)));

        services.AddDbContext<BannerReadDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString(AppSetting.ConnectionString)));


        services.AddScoped<IBannerCommandRepository, BannerCommandRepository>();
        services.AddScoped<IBannerQueryRepository, BannerQueryRepository>();



        return services;
    }







}
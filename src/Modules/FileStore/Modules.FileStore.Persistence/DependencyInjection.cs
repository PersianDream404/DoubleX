using Identity.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.FileStore.Application.Contract.DTOs.FileUploader;
using Modules.FileStore.Application.Contract.Interface.FileStores;
using Modules.FileStore.Persistence.Context;
using Modules.FileStore.Persistence.Repositories.FileStores;
using Modules.Product.Domain.Interface.FileStores;
using SharedKernel.Constants;

namespace Modules.FileStore.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
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

        services.Configure<FileStorageOptions>(options =>
             configuration.GetSection("Setting:FileStorage").Bind(options));
        #endregion


        services.AddDbContext<FileStoreWriteDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString(AppSetting.ConnectionString)));

        services.AddDbContext<FileStoreReadDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString(AppSetting.ConnectionString)));


        services.AddScoped<IFileStoreCommandRepository, FileStoreCommandRepository>();
        services.AddScoped<IFileStoreQueryRepository, FileStoreQueryRepository>();






        return services;
    }







}
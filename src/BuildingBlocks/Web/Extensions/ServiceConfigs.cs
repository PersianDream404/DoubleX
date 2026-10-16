using Asp.Versioning;
using CsharpGalaxy.LibraryExtension.Extensions.DateTimes;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using SharedKernel.Constants;
using SharedKernel.Events;
using SharedKernel.Events.Logs;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using Web.Helpers;
using Web.Infrastructure.Modules;


namespace Web.Extensions;

public static class ServiceConfigs
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddEndpointsConfiguration(configuration);
        services.AddSwaggerConfiguration();
        //  services.AddAuthenticationConfiguration(configuration);
        services.AddCorsConfiguration();
        services.AddApiVersioningConfiguration();

        services.AddScoped<IEventBus, MediatREventBus>();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(LogEventHandler).Assembly);
        });
    }



    private static void AddEndpointsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpoints(typeof(Program).Assembly);

        var modules = ModuleLoader.DiscoverModules();



        foreach (var module in modules)
        {
            module.RegisterServices(services, configuration);
        }

    }

    private static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {

            var buildDate = AssemblyInfoHelper.GetBuildDate();
            var version = AssemblyInfoHelper.GetAssemblyVersion();

            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = AppSetting.PrimaryName,
                Version = version,
                Description =
        $"آخرین آپدیت: {buildDate.ToShamsiDate()} - زمان: {buildDate:HH:mm:ss} - نسخه: {version}"
            });

            opt.CustomSchemaIds(type =>
                type.FullName?.Replace("+", "."));
            opt.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });

            opt.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });


        });
    }
    private static void AddApiVersioningConfiguration(this IServiceCollection services)
    {

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;


            // خواندن نسخه از هدر یا QueryString
            options.ApiVersionReader = ApiVersionReader.Combine(
                new HeaderApiVersionReader("Api-Version"),
                new QueryStringApiVersionReader("api-version")
            );
        });

    }
    private static void AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration.GetSection("Setting:SecretKey").Value!))
            };
        });
    }

    private static void AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(opt =>
        {
            opt.AddPolicy("CorsPolicy", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }

}


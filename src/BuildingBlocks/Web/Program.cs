using Infrastructure;
using Infrastructure.Context;
using Serilog;
using SharedKernel.Constants;
using System.Globalization;
using Modules.Personnel.Persistence.Extensions;
using Web.Extensions;
using Web.Infrastructure.Modules;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder
    .AddSqlServerDbContext<ApplicationDbContext>
        (AppSetting.ConnectionString);
builder.Services.AddSerilog((services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .ReadFrom.Services(services);
});


builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);
// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices(builder.Configuration);
var culture = new CultureInfo("fa-IR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var app = builder.Build();

#region Seed

// await app.Services.SeedSalonAsync();

#endregion

app.AddModuleDependencies();

app.UseSerilogRequestLogging();
app.MapMyEndpoints();
// Configure the HTTP request pipeline.
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Abolfazl and hamed");
    options.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");


app.MapGet("/test-logs", (ILogger<Program> logger) =>
{
    logger.LogInformation(
        "Test log generated at {Time}",
        DateTime.UtcNow
    );

    logger.LogWarning("This is a warning log");

    return Results.Ok(new
    {
        Message = "Logs generated successfully"
    });
});

app.Run();
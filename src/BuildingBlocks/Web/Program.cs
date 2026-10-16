using Infrastructure;
using Infrastructure.Context;
using SharedKernel.Constants;
using System.Globalization;
using Web.Extensions;
using Web.Infrastructure.Modules;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder
    .AddSqlServerDbContext<ApplicationDbContext>
    (AppSetting.ConnectionString);

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



app.Run();


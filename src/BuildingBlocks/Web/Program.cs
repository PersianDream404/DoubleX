using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Interface;
using SmeOpsHub.SharedKernel.Security;
using SmeOpsHub.Web.Infrastructure.Security;
using Web.Infrastructure.Modules;
using Web.Infrastructure.Navigation;

var builder = WebApplication.CreateBuilder(args);

//builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
builder.Services.AddInfrastructure(builder.Configuration);

var modules = ModuleLoader.DiscoverModules();

var mvcBuilder = builder.Services.AddControllersWithViews();

foreach (var module in modules)
{
    mvcBuilder.AddApplicationPart(module.GetType().Assembly);
}

//builder.Services.AddFluentValidationAutoValidation();
//builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

//builder.Services.AddScoped<SoftDeleteAuditInterceptor>();

//builder.Services.AddDbContext<AppDbContext>((sp, options) =>
//{
//    var connectionString = builder.Configuration.GetConnectionString("SmeOpsHub") ?? throw new InvalidOperationException("Connection string 'SmeOpsHub' not found");
//    options.UseSqlServer(connectionString);
//    options.AddInterceptors(sp.GetRequiredService<SoftDeleteAuditInterceptor>());
//});

var menuBuilder = new MenuBuilder();

foreach (var module in modules)
{
    module.RegisterServices(builder.Services, builder.Configuration);
    module.ConfigureMenu(menuBuilder);
}

builder.Services.AddSingleton<IModuleCatalog>(new ModuleCatalog(modules, menuBuilder.Items));
builder.Services.AddSingleton(modules);

//builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
//{
//    options.SignIn.RequireConfirmedAccount = false;

//    // reasonable defaults (tweak later)
//    options.Password.RequiredLength = 8;
//    options.Password.RequireDigit = true;
//    options.Password.RequireUppercase = true;
//    options.Password.RequireLowercase = true;
//    options.Password.RequireNonAlphanumeric = true;
//})
//    .AddEntityFrameworkStores<AppDbContext>()
//    .AddDefaultTokenProviders()
//    .AddDefaultUI();

builder.Services.Configure<PasswordHasherOptions>(options =>
{
    options.IterationCount = 600000;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AppPolicies.CanSoftDelete,
        p => p.RequireRole(AppRoles.Admin, AppRoles.Manager));

    options.AddPolicy(AppPolicies.CanApproveLeave,
        p => p.RequireRole(AppRoles.Admin, AppRoles.Manager));
});

builder.Services.AddRazorPages();

var app = builder.Build();

//await IdentitySeeder.SeedAsync(app.Services, app.Configuration);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/error/{0}");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

//app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapStaticAssets();

var moduleCatalog = app.Services.GetRequiredService<IModuleCatalog>();

foreach (var module in moduleCatalog.Modules)
{
    module.MapEndpoints(app);
}

app.MapAreaControllerRoute(name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

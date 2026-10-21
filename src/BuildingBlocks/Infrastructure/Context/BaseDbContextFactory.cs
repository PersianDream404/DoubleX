using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SharedKernel.Constants;

namespace Infrastructure.Context;

public class BaseDbContextFactory : IDesignTimeDbContextFactory<BaseDbContext>
{
    public BaseDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory()) // Or adjust path
           .AddJsonFile("appsettings.json")
           .Build();

        var connectionString = configuration.GetConnectionString(AppSetting.ConnectionString);

        var optionsBuilder = new DbContextOptionsBuilder<BaseDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new BaseDbContext(optionsBuilder.Options);
    }
}

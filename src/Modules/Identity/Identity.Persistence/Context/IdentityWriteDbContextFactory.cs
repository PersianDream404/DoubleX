using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SharedKernel.Constants;

namespace Identity.Persistence.Context;

public class IdentityWriteDbContextFactory : IDesignTimeDbContextFactory<IdentityWriteDbContext>
{
    public IdentityWriteDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityWriteDbContext>();
        optionsBuilder.UseSqlServer(AppSetting.ConnectionStringText);

        return new IdentityWriteDbContext(optionsBuilder.Options);
    }
}

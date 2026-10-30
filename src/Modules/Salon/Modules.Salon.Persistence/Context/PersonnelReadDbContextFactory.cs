using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SharedKernel.Constants;

namespace Modules.Personnel.Persistence.Context;

public class PersonnelReadDbContextFactory : IDesignTimeDbContextFactory<PersonnelReadDbContext>
{
    public PersonnelReadDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BaseDbContext>();
        optionsBuilder.UseSqlServer(AppSetting.ConnectionStringText);

        return new PersonnelReadDbContext(optionsBuilder.Options);
    }
}

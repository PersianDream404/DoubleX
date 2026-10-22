using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modules.Personnel.Domain.Entities;
using SharedKernel.Constants;

namespace Modules.Personnel.Persistence.Context;

public class PersonnelReadDbContext : BaseDbContext
{
    public PersonnelReadDbContext(DbContextOptions<BaseDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public DbSet<Domain.Entities.Personnel> Personnels { get; set; } = null!;
    public DbSet<Domain.Entities.Salon> Salons { get; set; } = null!;
}
public class PersonnelReadDbContextFactory : IDesignTimeDbContextFactory<PersonnelReadDbContext>
{
    public PersonnelReadDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BaseDbContext>();
        
        // اینجا کانکشن استرینگ دیتابیس خود را برای محیط توسعه (Development) قرار دهید
        optionsBuilder.UseSqlServer(AppSetting.ConnectionStringText);

        return new PersonnelReadDbContext(optionsBuilder.Options);
    }
}
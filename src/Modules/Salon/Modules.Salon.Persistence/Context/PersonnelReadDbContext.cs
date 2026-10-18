using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Modules.Personnel.Domain.Entities;

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
}

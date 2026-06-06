using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public abstract class IdentityApplicationDbContext : DbContext
{
    public IdentityApplicationDbContext(DbContextOptions options) : base(options)
    {
        //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    public void Commit()
    {
        SaveChangesAsync();
    }
}

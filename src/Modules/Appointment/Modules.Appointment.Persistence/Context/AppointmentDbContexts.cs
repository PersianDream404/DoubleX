using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modules.Appointment.Domain.Entities;
using AppointmentEntity = Modules.Appointment.Domain.Entities.Appointment;

namespace Modules.Appointment.Persistence.Context;

public sealed class AppointmentWriteDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Appointment");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppointmentWriteDbContext).Assembly);
    }

    public DbSet<AppointmentEntity> Appointments => Set<AppointmentEntity>();
}

public sealed class AppointmentReadDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Appointment");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppointmentReadDbContext).Assembly);
    }

    public DbSet<AppointmentEntity> Appointments => Set<AppointmentEntity>();
}

public sealed class AppointmentWriteDbContextFactory
    : IDesignTimeDbContextFactory<AppointmentWriteDbContext>
{
    public AppointmentWriteDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__BaseConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Set ConnectionStrings__BaseConnection to create Appointment migrations.");
        }

        var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        return new AppointmentWriteDbContext(options);
    }
}

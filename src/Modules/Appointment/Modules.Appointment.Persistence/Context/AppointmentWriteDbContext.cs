using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
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

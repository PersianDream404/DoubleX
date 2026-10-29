using System.Data;
using Microsoft.EntityFrameworkCore;
using Modules.Appointment.Domain.Entities;
using Modules.Appointment.Domain.Interfaces;
using Modules.Appointment.Persistence.Context;
using AppointmentEntity = Modules.Appointment.Domain.Entities.Appointment;

namespace Modules.Appointment.Persistence.Repositories;

public sealed class AppointmentCommandRepository(AppointmentWriteDbContext context)
    : IAppointmentCommandRepository
{
    public Task<AppointmentEntity?> GetTrackedAsync(
        long id,
        long salonId,
        CancellationToken cancellationToken) =>
        context.Appointments.SingleOrDefaultAsync(
            appointment => appointment.Id == id && appointment.SalonId == salonId,
            cancellationToken);

    public Task<AppointmentEntity?> GetByIdempotencyKeyAsync(
        long salonId,
        Guid idempotencyKey,
        CancellationToken cancellationToken) =>
        context.Appointments.SingleOrDefaultAsync(
            appointment => appointment.SalonId == salonId &&
                           appointment.IdempotencyKey == idempotencyKey,
            cancellationToken);

    public async Task<AppointmentWriteResult> AddIfAvailableAsync(
        AppointmentEntity appointment,
        CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var existing = await GetByIdempotencyKeyAsync(
            appointment.SalonId,
            appointment.IdempotencyKey,
            cancellationToken);
        if (existing is not null)
        {
            await transaction.RollbackAsync(cancellationToken);
            return existing.MatchesCreateRequest(
                appointment.CustomerId,
                appointment.ServiceId,
                appointment.ServiceTariffId,
                appointment.PersonnelId,
                appointment.StartsAt,
                appointment.DiscountAmount,
                appointment.Notes,
                appointment.NotificationChannel)
                ? AppointmentWriteResult.ExistingRequest
                : AppointmentWriteResult.IdempotencyKeyReused;
        }

        if (await HasConflictAsync(
                appointment.SalonId,
                appointment.PersonnelId,
                appointment.StartsAt,
                appointment.EndsAt,
                null,
                cancellationToken))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AppointmentWriteResult.TimeConflict;
        }

        await context.Appointments.AddAsync(appointment, cancellationToken);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return AppointmentWriteResult.Created;
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync(cancellationToken);
            context.ChangeTracker.Clear();
            var existingAfterFailure = await GetByIdempotencyKeyAsync(
                appointment.SalonId,
                appointment.IdempotencyKey,
                cancellationToken);
            if (existingAfterFailure is null)
            {
                if (await HasConflictAsync(
                        appointment.SalonId,
                        appointment.PersonnelId,
                        appointment.StartsAt,
                        appointment.EndsAt,
                        null,
                        cancellationToken))
                {
                    return AppointmentWriteResult.TimeConflict;
                }

                throw;
            }

            return existingAfterFailure.MatchesCreateRequest(
                appointment.CustomerId,
                appointment.ServiceId,
                appointment.ServiceTariffId,
                appointment.PersonnelId,
                appointment.StartsAt,
                appointment.DiscountAmount,
                appointment.Notes,
                appointment.NotificationChannel)
                ? AppointmentWriteResult.ExistingRequest
                : AppointmentWriteResult.IdempotencyKeyReused;
        }
    }

    public Task<bool> IsAvailableAsync(
        long salonId,
        long personnelId,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        long? excludingAppointmentId,
        CancellationToken cancellationToken) =>
        HasConflictAsync(
            salonId,
            personnelId,
            startsAt.ToUniversalTime(),
            endsAt.ToUniversalTime(),
            excludingAppointmentId,
            cancellationToken);

    public async Task<bool> UpdateScheduleIfAvailableAsync(
        AppointmentEntity appointment,
        CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);
        if (await HasConflictAsync(
                appointment.SalonId,
                appointment.PersonnelId,
                appointment.StartsAt,
                appointment.EndsAt,
                appointment.Id,
                cancellationToken))
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }

        try
        {
            if (!await SaveChangesAsync(cancellationToken))
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }

            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync(cancellationToken);
            context.ChangeTracker.Clear();
            if (await HasConflictAsync(
                    appointment.SalonId,
                    appointment.PersonnelId,
                    appointment.StartsAt,
                    appointment.EndsAt,
                    appointment.Id,
                    cancellationToken))
            {
                return false;
            }

            throw;
        }
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            context.ChangeTracker.Clear();
            return false;
        }
    }

    private Task<bool> HasConflictAsync(
        long salonId,
        long personnelId,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        long? excludingAppointmentId,
        CancellationToken cancellationToken) =>
        context.Appointments.AnyAsync(appointment =>
                appointment.SalonId == salonId &&
                appointment.PersonnelId == personnelId &&
                (appointment.AppointmentStatus == AppointmentStatus.Pending ||
                 appointment.AppointmentStatus == AppointmentStatus.Confirmed) &&
                appointment.StartsAt < endsAt &&
                appointment.EndsAt > startsAt &&
                (!excludingAppointmentId.HasValue ||
                 appointment.Id != excludingAppointmentId.Value),
            cancellationToken);
}

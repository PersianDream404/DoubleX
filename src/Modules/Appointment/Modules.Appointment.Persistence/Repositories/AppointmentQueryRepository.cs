using Framwork.PagedList;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Modules.Appointment.Application.Contract.DTOs.Appointments;
using Modules.Appointment.Application.Contract.Interfaces;
using Modules.Appointment.Domain.Entities;
using Modules.Appointment.Persistence.Context;
using Modules.Appointment.Persistence.Mapper;

namespace Modules.Appointment.Persistence.Repositories;

public sealed class AppointmentQueryRepository(AppointmentReadDbContext context)
    : IAppointmentQueryRepository
{
    public async Task<PagedList<AppointmentListItemDto>> GetAllAsync(
        GetAppointmentsRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = context.Appointments
            .AsNoTracking()
            .Where(appointment => appointment.SalonId == request.SalonId);

        if (request.CustomerId.HasValue)
            query = query.Where(appointment => appointment.CustomerId == request.CustomerId);
        if (request.PersonnelId.HasValue)
            query = query.Where(appointment => appointment.PersonnelId == request.PersonnelId);
        if (request.ServiceId.HasValue)
            query = query.Where(appointment => appointment.ServiceId == request.ServiceId);
        if (request.Status.HasValue)
            query = query.Where(appointment => appointment.AppointmentStatus == request.Status);
        if (request.From.HasValue)
        {
            var from = request.From.Value.ToUniversalTime();
            query = query.Where(appointment => appointment.StartsAt >= from);
        }
        if (request.To.HasValue)
        {
            var to = request.To.Value.ToUniversalTime();
            query = query.Where(appointment => appointment.StartsAt < to);
        }

        return await query
            .OrderBy(appointment => appointment.StartsAt)
            .Select(AppointmentMapper.ToListItemDto())
            .ToPagedList(request.PageNumber, request.PageSize, cancellationToken);
    }

    public Task<AppointmentDetailsDto?> GetByIdAsync(
        long id,
        long salonId,
        CancellationToken cancellationToken) =>
        context.Appointments
            .AsNoTracking()
            .Where(appointment => appointment.Id == id && appointment.SalonId == salonId)
            .Select(AppointmentMapper.ToDetailsDto())
            .FirstOrDefaultAsync(cancellationToken);
}

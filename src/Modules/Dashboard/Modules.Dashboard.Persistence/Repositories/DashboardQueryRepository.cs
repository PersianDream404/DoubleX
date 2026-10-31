using Modules.Appointment.Application.Contract.DTOs.Appointments;
using Modules.Appointment.Application.Contract.Interfaces;
using Modules.Appointment.Domain.Entities;
using Modules.Dashboard.Application.Contract.DTOs;
using Modules.Dashboard.Application.Contract.Interfaces;
using Modules.Finance.Application.Contract.Interfaces;
using Modules.Personnel.Application.Contract.Interfaces;
using Modules.Reception.Domain.Entities;
using Modules.Reception.Domain.Interfaces;
using Modules.Reception.Domain.ReadModels;

namespace Modules.Dashboard.Persistence.Repositories;

public sealed class DashboardQueryRepository(
    IReceptionReportQueryRepository receptionRepository,
    IAppointmentQueryRepository appointmentRepository,
    IFinanceQueryRepository financeRepository,
    IPersonnelQueryRepository personnelRepository)
    : IDashboardQueryRepository
{
    public async Task<DashboardSummaryDto> GetSummaryAsync(
        long salonId,
        DateTimeOffset from,
        DateTimeOffset toExclusive,
        CancellationToken cancellationToken)
    {
        var receptionTask = receptionRepository.GetSummaryAsync(
            CreateFilter(salonId, from, toExclusive),
            cancellationToken);
        var financeTask = financeRepository.GetSummaryAsync(
            salonId,
            from,
            toExclusive.AddTicks(-1),
            cancellationToken);
        await Task.WhenAll(receptionTask, financeTask);

        var receptions = await receptionTask;
        var finance = await financeTask;
        return new DashboardSummaryDto(
            receptions.ReceptionCount,
            receptions.CompletedReceptionCount,
            receptions.InProgressReceptionCount,
            receptions.CancelledReceptionCount,
            receptions.AppointmentReceptionCount,
            receptions.WalkInReceptionCount,
            receptions.UniqueCustomerCount,
            receptions.ServiceSubtotal,
            receptions.DiscountAmount,
            receptions.ServiceFinalAmount,
            receptions.PaidAmount,
            receptions.RemainingAmount,
            finance.RecordedExpenses,
            finance.CancelledExpenses);
    }

    public async Task<DashboardTodayAppointmentsDto> GetTodayAppointmentsAsync(
        long salonId,
        DateTimeOffset from,
        DateTimeOffset toExclusive,
        CancellationToken cancellationToken)
    {
        var appointments = await appointmentRepository.GetAllAsync(
            new GetAppointmentsRequestDto
            {
                SalonId = salonId,
                From = from,
                To = toExclusive,
                PageNumber = 1,
                PageSize = 100
            },
            cancellationToken);
        var appointmentIds = appointments.List
            .Select(appointment => appointment.Id)
            .ToArray();
        var receptionLinks = await receptionRepository.GetAppointmentLinksAsync(
            salonId,
            appointmentIds,
            cancellationToken);
        var receptionIdsByAppointment = receptionLinks
            .GroupBy(link => link.AppointmentId)
            .ToDictionary(group => group.Key, group => group.First().ReceptionId);

        var items = appointments.List
            .Select(appointment => new DashboardAppointmentDto(
                appointment.Id,
                receptionIdsByAppointment.GetValueOrDefault(appointment.Id),
                appointment.CustomerName,
                appointment.ServiceName,
                appointment.PersonnelName,
                appointment.StartsAt,
                appointment.EndsAt,
                appointment.FinalAmount,
                appointment.Status.ToString()))
            .ToArray();
        return new DashboardTodayAppointmentsDto(items, appointments.Pagination.TotalCount);
    }

    public async Task<IReadOnlyList<DashboardAlertDto>> GetAlertsAsync(
        long salonId,
        DateOnly today,
        CancellationToken cancellationToken)
    {
        var from = StartOfDay(today);
        var toExclusive = StartOfDay(today.AddDays(1));
        var pendingAppointmentsTask = appointmentRepository.GetAllAsync(
            new GetAppointmentsRequestDto
            {
                SalonId = salonId,
                Status = AppointmentStatus.Pending,
                From = from,
                To = toExclusive,
                PageNumber = 1,
                PageSize = 50
            },
            cancellationToken);
        var debtsTask = receptionRepository.GetOutstandingDebtSummaryAsync(
            salonId,
            cancellationToken);
        var expiringContractsTask = personnelRepository.GetExpiringContractsAsync(
            salonId,
            today,
            today.AddDays(30),
            cancellationToken);

        await Task.WhenAll(
            pendingAppointmentsTask,
            debtsTask,
            expiringContractsTask);

        var pendingAppointments = await pendingAppointmentsTask;
        var debts = await debtsTask;
        var expiringContracts = await expiringContractsTask;
        var alerts = new List<DashboardAlertDto>(3);

        if (pendingAppointments.Pagination.TotalCount > 0)
        {
            alerts.Add(new DashboardAlertDto(
                DashboardAlertType.PendingAppointments,
                "نوبت‌های در انتظار تأیید امروز",
                pendingAppointments.Pagination.TotalCount,
                null,
                pendingAppointments.List.Select(item => item.Id).ToArray()));
        }

        if (debts.ReceptionCount > 0)
        {
            alerts.Add(new DashboardAlertDto(
                DashboardAlertType.OutstandingDebts,
                "بدهی‌های پرداخت‌نشده",
                debts.ReceptionCount,
                debts.RemainingAmount,
                []));
        }

        if (expiringContracts.TotalCount > 0)
        {
            alerts.Add(new DashboardAlertDto(
                DashboardAlertType.ExpiringPersonnelContracts,
                "قراردادهای پرسنلی نزدیک به پایان",
                expiringContracts.TotalCount,
                null,
                expiringContracts.Items
                    .Select(item => item.PersonnelId)
                    .Distinct()
                    .ToArray()));
        }

        return alerts;
    }

    public async Task<DashboardChartsDto> GetChartsAsync(
        long salonId,
        DateTimeOffset from,
        DateTimeOffset toExclusive,
        CancellationToken cancellationToken)
    {
        var filter = CreateFilter(salonId, from, toExclusive);
        var financeTask = financeRepository.GetSummaryAsync(
            salonId,
            from,
            toExclusive.AddTicks(-1),
            cancellationToken);

        var summary = await receptionRepository.GetSummaryAsync(filter, cancellationToken);
        var services = await receptionRepository.GetServicesAsync(
            filter with { PageSize = 50, ServiceStatus = ReceptionServiceStatus.Completed },
            cancellationToken);
        var personnel = await receptionRepository.GetPersonnelAsync(
            filter with { PageSize = 50, ServiceStatus = ReceptionServiceStatus.Completed },
            cancellationToken);
        var personnelIds = personnel.Page.List
            .Select(item => item.PersonnelId)
            .Distinct()
            .ToArray();
        var personnelLookups = await personnelRepository.GetReceptionLookupsAsync(
            salonId,
            personnelIds,
            cancellationToken);
        var personnelNames = personnelLookups.ToDictionary(item => item.Id, item => item.FullName);
        var finance = await financeTask;

        return new DashboardChartsDto(
            summary.Trend
                .Select(item => new DashboardFinancialTrendDto(
                    item.Date,
                    item.ReceptionCount,
                    item.ServiceSubtotal,
                    item.DiscountAmount,
                    item.ServiceFinalAmount,
                    item.PaidAmount))
                .ToArray(),
            finance.RecordedExpenses,
            finance.CancelledExpenses,
            services.Page.List
                .Select(item => new DashboardPopularServiceDto(
                    item.ServiceId,
                    item.ServiceName,
                    item.CompletedServiceCount,
                    item.ServiceFinalAmount))
                .ToArray(),
            personnel.Page.List
                .Select(item => new DashboardPersonnelPerformanceDto(
                    item.PersonnelId,
                    personnelNames.GetValueOrDefault(item.PersonnelId),
                    item.CompletedServiceCount,
                    item.ServiceFinalAmount))
                .ToArray());
    }

    private static ReceptionReportFilter CreateFilter(
        long salonId,
        DateTimeOffset from,
        DateTimeOffset toExclusive) =>
        new(
            salonId,
            from,
            toExclusive,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            1,
            50,
            false);

    private static DateTimeOffset StartOfDay(DateOnly date) =>
        new(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
}

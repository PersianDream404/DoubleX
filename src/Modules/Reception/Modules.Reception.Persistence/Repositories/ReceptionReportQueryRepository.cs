using Framwork.PagedList;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Modules.Reception.Domain.Entities;
using Modules.Reception.Domain.Interfaces;
using Modules.Reception.Domain.ReadModels;
using Modules.Reception.Persistence.Context;
using ReceptionEntity = Modules.Reception.Domain.Entities.Reception;

namespace Modules.Reception.Persistence.Repositories;

public sealed class ReceptionReportQueryRepository(ReceptionReadDbContext context)
    : IReceptionReportQueryRepository
{
    public async Task<ReceptionReportSummaryReadModel> GetSummaryAsync(
        ReceptionReportFilter filter,
        CancellationToken cancellationToken)
    {
        var allReceptions = FilterReceptions(filter);
        var billableReceptions = allReceptions.Where(item =>
            item.ReceptionStatus != ReceptionStatus.Cancelled);

        var counts = await allReceptions
            .GroupBy(_ => 1)
            .Select(group => new
            {
                ReceptionCount = group.Count(),
                CompletedReceptionCount = group.Count(item =>
                    item.ReceptionStatus == ReceptionStatus.Completed),
                InProgressReceptionCount = group.Count(item =>
                    item.ReceptionStatus == ReceptionStatus.InProgress),
                CancelledReceptionCount = group.Count(item =>
                    item.ReceptionStatus == ReceptionStatus.Cancelled)
            })
            .SingleOrDefaultAsync(cancellationToken);

        var serviceQuery =
            from service in context.ReceptionServices.AsNoTracking()
            join reception in billableReceptions
                on service.ReceptionId equals reception.Id
            where service.ServiceStatus != ReceptionServiceStatus.Cancelled
            select new { reception.ReceptionDate, Service = service };

        var serviceTotals = await serviceQuery
            .GroupBy(_ => 1)
            .Select(group => new
            {
                ServiceSubtotal = group.Sum(item =>
                    item.Service.Quantity * item.Service.UnitPrice),
                DiscountAmount = group.Sum(item => item.Service.DiscountAmount),
                ServiceFinalAmount = group.Sum(item => item.Service.FinalAmount)
            })
            .SingleOrDefaultAsync(cancellationToken);

        var materialChargeAmount = await (
                from material in context.ReceptionServiceMaterials.AsNoTracking()
                join service in context.ReceptionServices.AsNoTracking()
                    on material.ReceptionServiceId equals service.Id
                join reception in billableReceptions
                    on service.ReceptionId equals reception.Id
                where material.MovementRegistered && !material.MovementReversed
                select (decimal?)material.TotalCost)
            .SumAsync(cancellationToken) ?? 0m;

        var payments =
            from payment in context.ReceptionPayments.AsNoTracking()
            join reception in billableReceptions
                on payment.ReceptionId equals reception.Id
            where payment.PaymentStatus == ReceptionPaymentStatus.Succeeded
            select new { reception.ReceptionDate, payment.Amount, payment.PaymentMethod };

        var paidAmount = await payments
            .Select(item => (decimal?)item.Amount)
            .SumAsync(cancellationToken) ?? 0m;

        var remainingAmount = await billableReceptions
            .Select(item => (decimal?)(
                item.TotalAmount -
                (item.Payments
                    .Where(payment => payment.PaymentStatus == ReceptionPaymentStatus.Succeeded)
                    .Sum(payment => (decimal?)payment.Amount) ?? 0m)))
            .SumAsync(cancellationToken) ?? 0m;

        var receptionTrend = await billableReceptions
            .GroupBy(item => item.ReceptionDate.Date)
            .Select(group => new
            {
                Date = group.Key,
                ReceptionCount = group.Count()
            })
            .ToListAsync(cancellationToken);

        var serviceTrend = await serviceQuery
            .GroupBy(item => item.ReceptionDate.Date)
            .Select(group => new
            {
                Date = group.Key,
                ServiceSubtotal = group.Sum(item =>
                    item.Service.Quantity * item.Service.UnitPrice),
                DiscountAmount = group.Sum(item => item.Service.DiscountAmount),
                ServiceFinalAmount = group.Sum(item => item.Service.FinalAmount)
            })
            .ToListAsync(cancellationToken);

        var paymentTrend = await payments
            .GroupBy(item => item.ReceptionDate.Date)
            .Select(group => new
            {
                Date = group.Key,
                PaidAmount = group.Sum(item => item.Amount)
            })
            .ToListAsync(cancellationToken);

        var trendByDate = receptionTrend.ToDictionary(
            item => item.Date,
            item => new TrendAccumulator(item.ReceptionCount));
        foreach (var item in serviceTrend)
        {
            if (!trendByDate.TryGetValue(item.Date, out var trend))
            {
                trend = new TrendAccumulator(0);
                trendByDate.Add(item.Date, trend);
            }

            trend.ServiceSubtotal = item.ServiceSubtotal;
            trend.DiscountAmount = item.DiscountAmount;
            trend.ServiceFinalAmount = item.ServiceFinalAmount;
        }

        foreach (var item in paymentTrend)
        {
            if (!trendByDate.TryGetValue(item.Date, out var trend))
            {
                trend = new TrendAccumulator(0);
                trendByDate.Add(item.Date, trend);
            }

            trend.PaidAmount = item.PaidAmount;
        }

        var paymentMethodTotals = await payments
            .GroupBy(item => item.PaymentMethod)
            .Select(group => new
            {
                PaymentMethod = group.Key,
                Amount = group.Sum(item => item.Amount)
            })
            .OrderByDescending(item => item.Amount)
            .ToListAsync(cancellationToken);
        var paymentMethods = paymentMethodTotals
            .Select(item => new ReceptionReportPaymentMethodReadModel(
                item.PaymentMethod,
                item.Amount))
            .ToArray();

        return new ReceptionReportSummaryReadModel(
            counts?.ReceptionCount ?? 0,
            counts?.CompletedReceptionCount ?? 0,
            counts?.InProgressReceptionCount ?? 0,
            counts?.CancelledReceptionCount ?? 0,
            serviceTotals?.ServiceSubtotal ?? 0m,
            serviceTotals?.DiscountAmount ?? 0m,
            serviceTotals?.ServiceFinalAmount ?? 0m,
            paidAmount,
            remainingAmount,
            materialChargeAmount,
            trendByDate
                .OrderBy(item => item.Key)
                .Select(item => new ReceptionReportTrendReadModel(
                    DateOnly.FromDateTime(item.Key),
                    item.Value.ReceptionCount,
                    item.Value.ServiceSubtotal,
                    item.Value.DiscountAmount,
                    item.Value.ServiceFinalAmount,
                    item.Value.PaidAmount))
                .ToArray(),
            paymentMethods);
    }

    public async Task<ReceptionReportPageReadModel<ReceptionReportReceptionReadModel, ReceptionReportTotalsReadModel>>
        GetReceptionsAsync(ReceptionReportFilter filter, CancellationToken cancellationToken)
    {
        var query = FilterReceptions(filter);
        var orderedQuery = filter.OldestFirst
            ? query.OrderBy(reception => reception.ReceptionDate)
            : query.OrderByDescending(reception => reception.ReceptionDate);
        var rows = orderedQuery.Select(reception => new ReceptionReportReceptionReadModel(
            reception.Id,
            reception.ReceptionNumber,
            reception.CustomerId,
            reception.CustomerName,
            reception.ReceptionistId,
            reception.ReceptionDate,
            reception.ReceptionStatus,
            reception.ReceptionFinancialStatus,
            reception.Services.Count(service =>
                (!filter.ServiceId.HasValue || service.ServiceId == filter.ServiceId.Value) &&
                (!filter.PersonnelId.HasValue ||
                 service.PersonnelId == filter.PersonnelId.Value) &&
                (!filter.MaterialId.HasValue ||
                 service.Materials.Any(material =>
                     material.MaterialId == filter.MaterialId.Value)) &&
                (!filter.ServiceStatus.HasValue ||
                 service.ServiceStatus == filter.ServiceStatus.Value)),
            reception.Services
                .Where(service =>
                                  (!filter.ServiceId.HasValue ||
                                   service.ServiceId == filter.ServiceId.Value) &&
                                  (!filter.PersonnelId.HasValue ||
                                   service.PersonnelId == filter.PersonnelId.Value) &&
                                  (!filter.MaterialId.HasValue ||
                                   service.Materials.Any(material =>
                                       material.MaterialId == filter.MaterialId.Value)) &&
                                  (!filter.ServiceStatus.HasValue ||
                                   service.ServiceStatus == filter.ServiceStatus.Value) &&
                                  service.ServiceStatus != ReceptionServiceStatus.Cancelled)
                .Sum(service => (decimal?)(
                    service.Quantity * service.UnitPrice)) ?? 0m,
            reception.Services
                .Where(service =>
                                  (!filter.ServiceId.HasValue ||
                                   service.ServiceId == filter.ServiceId.Value) &&
                                  (!filter.PersonnelId.HasValue ||
                                   service.PersonnelId == filter.PersonnelId.Value) &&
                                  (!filter.MaterialId.HasValue ||
                                   service.Materials.Any(material =>
                                       material.MaterialId == filter.MaterialId.Value)) &&
                                  (!filter.ServiceStatus.HasValue ||
                                   service.ServiceStatus == filter.ServiceStatus.Value) &&
                                  service.ServiceStatus != ReceptionServiceStatus.Cancelled)
                .Sum(service => (decimal?)service.DiscountAmount) ?? 0m,
            reception.Services
                .Where(service =>
                                  (!filter.ServiceId.HasValue ||
                                   service.ServiceId == filter.ServiceId.Value) &&
                                  (!filter.PersonnelId.HasValue ||
                                   service.PersonnelId == filter.PersonnelId.Value) &&
                                  (!filter.MaterialId.HasValue ||
                                   service.Materials.Any(material =>
                                       material.MaterialId == filter.MaterialId.Value)) &&
                                  (!filter.ServiceStatus.HasValue ||
                                   service.ServiceStatus == filter.ServiceStatus.Value) &&
                                  service.ServiceStatus != ReceptionServiceStatus.Cancelled)
                .Sum(service => (decimal?)service.FinalAmount) ?? 0m,
            reception.Payments
                .Where(payment => payment.PaymentStatus == ReceptionPaymentStatus.Succeeded)
                .Sum(payment => (decimal?)payment.Amount) ?? 0m,
            reception.TotalAmount -
            (reception.Payments
                .Where(payment => payment.PaymentStatus == ReceptionPaymentStatus.Succeeded)
                .Sum(payment => (decimal?)payment.Amount) ?? 0m)));

        var totalsProjection = query.Select(reception => new
        {
            reception.Services,
            PaidAmount = reception.Payments
                .Where(payment => payment.PaymentStatus == ReceptionPaymentStatus.Succeeded)
                .Sum(payment => (decimal?)payment.Amount) ?? 0m,
            RemainingAmount = reception.TotalAmount -
                (reception.Payments
                    .Where(payment => payment.PaymentStatus == ReceptionPaymentStatus.Succeeded)
                    .Sum(payment => (decimal?)payment.Amount) ?? 0m)
        });
        var totals = await totalsProjection
            .GroupBy(_ => 1)
            .Select(group => new ReceptionReportTotalsReadModel(
                group.Count(),
                group.Sum(item => item.Services
                    .Where(service =>
                                      (!filter.ServiceId.HasValue ||
                                       service.ServiceId == filter.ServiceId.Value) &&
                                      (!filter.PersonnelId.HasValue ||
                                       service.PersonnelId == filter.PersonnelId.Value) &&
                                      (!filter.MaterialId.HasValue ||
                                       service.Materials.Any(material =>
                                           material.MaterialId == filter.MaterialId.Value)) &&
                                      (!filter.ServiceStatus.HasValue ||
                                       service.ServiceStatus == filter.ServiceStatus.Value) &&
                                      service.ServiceStatus != ReceptionServiceStatus.Cancelled)
                    .Sum(service => (decimal?)(service.Quantity * service.UnitPrice)) ?? 0m),
                group.Sum(item => item.Services
                    .Where(service =>
                                      (!filter.ServiceId.HasValue ||
                                       service.ServiceId == filter.ServiceId.Value) &&
                                      (!filter.PersonnelId.HasValue ||
                                       service.PersonnelId == filter.PersonnelId.Value) &&
                                      (!filter.MaterialId.HasValue ||
                                       service.Materials.Any(material =>
                                           material.MaterialId == filter.MaterialId.Value)) &&
                                      (!filter.ServiceStatus.HasValue ||
                                       service.ServiceStatus == filter.ServiceStatus.Value) &&
                                      service.ServiceStatus != ReceptionServiceStatus.Cancelled)
                    .Sum(service => (decimal?)service.DiscountAmount) ?? 0m),
                group.Sum(item => item.Services
                    .Where(service =>
                                      (!filter.ServiceId.HasValue ||
                                       service.ServiceId == filter.ServiceId.Value) &&
                                      (!filter.PersonnelId.HasValue ||
                                       service.PersonnelId == filter.PersonnelId.Value) &&
                                      (!filter.MaterialId.HasValue ||
                                       service.Materials.Any(material =>
                                           material.MaterialId == filter.MaterialId.Value)) &&
                                      (!filter.ServiceStatus.HasValue ||
                                       service.ServiceStatus == filter.ServiceStatus.Value) &&
                                      service.ServiceStatus != ReceptionServiceStatus.Cancelled)
                    .Sum(service => (decimal?)service.FinalAmount) ?? 0m),
                group.Sum(item => item.PaidAmount),
                group.Sum(item => item.RemainingAmount)))
            .SingleOrDefaultAsync(cancellationToken);

        var page = await rows
            .ToPagedList(filter.PageNumber, filter.PageSize, cancellationToken);

        return new ReceptionReportPageReadModel<ReceptionReportReceptionReadModel, ReceptionReportTotalsReadModel>(
            page,
            totals ?? new ReceptionReportTotalsReadModel(0, 0m, 0m, 0m, 0m, 0m));
    }

    public async Task<ReceptionReportPageReadModel<ReceptionReportPersonnelReadModel, ReceptionReportTotalsReadModel>>
        GetPersonnelAsync(ReceptionReportFilter filter, CancellationToken cancellationToken)
    {
        var query = FilterServices(filter, defaultToCompleted: true)
            .Where(service => service.PersonnelId.HasValue);
        var grouped = query
            .GroupBy(service => service.PersonnelId!.Value)
            .Select(group => new ReceptionReportPersonnelReadModel(
                group.Key,
                group.Count(),
                group.Select(service => service.Reception.Id).Distinct().Count(),
                group.Sum(service => service.Quantity * service.UnitPrice),
                group.Sum(service => service.DiscountAmount),
                group.Sum(service => service.FinalAmount)));
        var pageData = await grouped.ToListAsync(cancellationToken);
        var orderedData = pageData.OrderByDescending(item => item.ServiceFinalAmount).ToList();
        var pageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;
        var pageSize = filter.PageSize > 0 ? filter.PageSize : 50;
        var totalCount = orderedData.Count;
        var pagedData = orderedData
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var page = new Framwork.PagedList.PagedList<ReceptionReportPersonnelReadModel>
        {
            List = pagedData,
            Pagination = new Framwork.PagedList.PagedListInfo
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };
        var totals = await query
            .GroupBy(_ => 1)
            .Select(group => new ReceptionReportTotalsReadModel(
                group.Select(service => service.Reception.Id).Distinct().Count(),
                group.Sum(service => service.Quantity * service.UnitPrice),
                group.Sum(service => service.DiscountAmount),
                group.Sum(service => service.FinalAmount),
                0m,
                0m))
            .SingleOrDefaultAsync(cancellationToken);

        return new ReceptionReportPageReadModel<ReceptionReportPersonnelReadModel, ReceptionReportTotalsReadModel>(
            page,
            totals ?? new ReceptionReportTotalsReadModel(0, 0m, 0m, 0m, 0m, 0m));
    }

    public async Task<ReceptionReportPageReadModel<ReceptionReportServiceReadModel, ReceptionReportTotalsReadModel>>
        GetServicesAsync(ReceptionReportFilter filter, CancellationToken cancellationToken)
    {
        var query = FilterServices(filter, defaultToCompleted: true);
        var grouped = query
            .GroupBy(service => new { service.ServiceId, service.ServiceName })
            .Select(group => new ReceptionReportServiceReadModel(
                group.Key.ServiceId,
                group.Key.ServiceName,
                group.Count(),
                group.Select(service => service.Reception.Id).Distinct().Count(),
                group.Sum(service => service.Quantity * service.UnitPrice),
                group.Sum(service => service.DiscountAmount),
                group.Sum(service => service.FinalAmount),
                group.Sum(service => service.FinalAmount) / group.Count()));
        var pageData = await grouped.ToListAsync(cancellationToken);
        var orderedData = pageData.OrderByDescending(item => item.ServiceFinalAmount).ToList();
        var pageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;
        var pageSize = filter.PageSize > 0 ? filter.PageSize : 50;
        var totalCount = orderedData.Count;
        var pagedData = orderedData
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var page = new Framwork.PagedList.PagedList<ReceptionReportServiceReadModel>
        {
            List = pagedData,
            Pagination = new Framwork.PagedList.PagedListInfo
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };
        var totals = await query
            .GroupBy(_ => 1)
            .Select(group => new ReceptionReportTotalsReadModel(
                group.Select(service => service.Reception.Id).Distinct().Count(),
                group.Sum(service => service.Quantity * service.UnitPrice),
                group.Sum(service => service.DiscountAmount),
                group.Sum(service => service.FinalAmount),
                0m,
                0m))
            .SingleOrDefaultAsync(cancellationToken);

        return new ReceptionReportPageReadModel<ReceptionReportServiceReadModel, ReceptionReportTotalsReadModel>(
            page,
            totals ?? new ReceptionReportTotalsReadModel(0, 0m, 0m, 0m, 0m, 0m));
    }

    public async Task<ReceptionReportPageReadModel<ReceptionReportDebtReadModel, ReceptionReportDebtTotalsReadModel>>
        GetDebtsAsync(ReceptionReportFilter filter, CancellationToken cancellationToken)
    {
        var baseQuery = FilterReceptions(filter)
            .Where(item => item.ReceptionStatus != ReceptionStatus.Cancelled)
            .Select(reception => new ReceptionReportDebtReadModel(
                reception.Id,
                reception.ReceptionNumber,
                reception.CustomerId,
                reception.CustomerName,
                reception.ReceptionDate,
                reception.ReceptionStatus,
                reception.TotalAmount,
                reception.Payments
                    .Where(payment => payment.PaymentStatus == ReceptionPaymentStatus.Succeeded)
                    .Sum(payment => (decimal?)payment.Amount) ?? 0m,
                reception.TotalAmount -
                (reception.Payments
                    .Where(payment => payment.PaymentStatus == ReceptionPaymentStatus.Succeeded)
                    .Sum(payment => (decimal?)payment.Amount) ?? 0m)));
        var allDebts = await baseQuery.ToListAsync(cancellationToken);
        var query = allDebts
            .Where(item => item.RemainingAmount > 0m)
            .Where(item => !filter.MinimumRemainingAmount.HasValue ||
                           item.RemainingAmount >= filter.MinimumRemainingAmount.Value)
            .Where(item => !filter.MaximumRemainingAmount.HasValue ||
                           item.RemainingAmount <= filter.MaximumRemainingAmount.Value)
            .ToList();

        var totals = query
            .GroupBy(_ => 1)
            .Select(group => new ReceptionReportDebtTotalsReadModel(
                group.Count(),
                group.Sum(item => item.TotalAmount),
                group.Sum(item => item.PaidAmount),
                group.Sum(item => item.RemainingAmount)))
            .FirstOrDefault() ?? new ReceptionReportDebtTotalsReadModel(0, 0m, 0m, 0m);
        var orderedData = filter.OldestFirst
            ? query.OrderBy(item => item.ReceptionDate).ToList()
            : query.OrderByDescending(item => item.RemainingAmount)
                .ThenBy(item => item.ReceptionDate).ToList();
        var pageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;
        var pageSize = filter.PageSize > 0 ? filter.PageSize : 50;
        var totalCount = orderedData.Count;
        var pagedData = orderedData
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var page = new Framwork.PagedList.PagedList<ReceptionReportDebtReadModel>
        {
            List = pagedData,
            Pagination = new Framwork.PagedList.PagedListInfo
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };

        return new ReceptionReportPageReadModel<ReceptionReportDebtReadModel, ReceptionReportDebtTotalsReadModel>(
            page,
            totals);
    }

    public async Task<ReceptionReportPageReadModel<ReceptionReportDiscountReadModel, ReceptionReportDiscountTotalsReadModel>>
        GetDiscountsAsync(ReceptionReportFilter filter, CancellationToken cancellationToken)
    {
        var baseQuery = FilterServices(filter, defaultToCompleted: false)
            .Where(service =>
                service.ServiceStatus != ReceptionServiceStatus.Cancelled &&
                service.DiscountAmount > 0m)
            .Select(service => new ReceptionReportDiscountReadModel(
                service.Reception.Id,
                service.Reception.ReceptionNumber,
                service.Reception.CustomerId,
                service.Reception.CustomerName,
                service.Reception.ReceptionDate,
                service.ServiceId,
                service.ServiceName,
                service.Quantity * service.UnitPrice,
                service.DiscountAmount,
                service.FinalAmount));
        var query = await baseQuery.ToListAsync(cancellationToken);

        var totals = new ReceptionReportDiscountTotalsReadModel(
            query.Count,
            query.Sum(item => item.ServiceSubtotal),
            query.Sum(item => item.DiscountAmount),
            query.Sum(item => item.ServiceFinalAmount));
        var orderedData = query
            .OrderByDescending(item => item.ReceptionDate)
            .ThenBy(item => item.ReceptionNumber)
            .ToList();
        var pageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;
        var pageSize = filter.PageSize > 0 ? filter.PageSize : 50;
        var totalCount = orderedData.Count;
        var pagedData = orderedData
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var page = new Framwork.PagedList.PagedList<ReceptionReportDiscountReadModel>
        {
            List = pagedData,
            Pagination = new Framwork.PagedList.PagedListInfo
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };

        return new ReceptionReportPageReadModel<ReceptionReportDiscountReadModel, ReceptionReportDiscountTotalsReadModel>(
            page,
            totals);
    }

    public async Task<ReceptionReportPageReadModel<ReceptionReportMaterialReadModel, ReceptionReportMaterialTotalsReadModel>>
        GetMaterialsAsync(ReceptionReportFilter filter, CancellationToken cancellationToken)
    {
        var query =
            from material in context.ReceptionServiceMaterials.AsNoTracking()
            join service in context.ReceptionServices.AsNoTracking()
                on material.ReceptionServiceId equals service.Id
            join reception in FilterReceptions(filter, includeServiceFilters: false)
                on service.ReceptionId equals reception.Id
            where material.MovementRegistered &&
                  !material.MovementReversed &&
                  reception.ReceptionStatus != ReceptionStatus.Cancelled &&
                  service.ServiceStatus != ReceptionServiceStatus.Cancelled &&
                  (!filter.ServiceStatus.HasValue ||
                   service.ServiceStatus == filter.ServiceStatus.Value) &&
                  (!filter.ServiceId.HasValue || service.ServiceId == filter.ServiceId.Value) &&
                  (!filter.PersonnelId.HasValue ||
                   service.PersonnelId == filter.PersonnelId.Value) &&
                  (!filter.MaterialId.HasValue || material.MaterialId == filter.MaterialId.Value)
            select new { Material = material, Service = service };

        var grouped = query
            .GroupBy(item => new
            {
                item.Material.MaterialId,
                item.Service.ServiceId,
                item.Service.ServiceName
            })
            .Select(group => new ReceptionReportMaterialReadModel(
                group.Key.MaterialId,
                group.Key.ServiceId,
                group.Key.ServiceName,
                group.Count(),
                group.Sum(item => item.Material.Quantity),
                group.Sum(item => (decimal?)item.Material.TotalCost)));
        var totals = await query
            .GroupBy(_ => 1)
            .Select(group => new ReceptionReportMaterialTotalsReadModel(
                group.Count(),
                group.Sum(item => item.Material.Quantity),
                group.Sum(item => (decimal?)item.Material.TotalCost) ?? 0m))
            .SingleOrDefaultAsync(cancellationToken);
        var pageData = await grouped.ToListAsync(cancellationToken);
        var orderedData = pageData
            .OrderByDescending(item => item.Quantity)
            .ThenBy(item => item.MaterialId)
            .ToList();
        var pageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;
        var pageSize = filter.PageSize > 0 ? filter.PageSize : 50;
        var totalCount = orderedData.Count;
        var pagedData = orderedData
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var page = new Framwork.PagedList.PagedList<ReceptionReportMaterialReadModel>
        {
            List = pagedData,
            Pagination = new Framwork.PagedList.PagedListInfo
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };

        return new ReceptionReportPageReadModel<ReceptionReportMaterialReadModel, ReceptionReportMaterialTotalsReadModel>(
            page,
            totals ?? new ReceptionReportMaterialTotalsReadModel(0, 0m, 0m));
    }

    private IQueryable<ReceptionEntity> FilterReceptions(
        ReceptionReportFilter filter,
        bool includeServiceFilters = true)
    {
        var query = context.Receptions
            .AsNoTracking()
            .Where(item => item.SalonId == filter.SalonId);

        if (filter.FromDate.HasValue)
        {
            query = query.Where(item => item.ReceptionDate >= filter.FromDate.Value);
        }

        if (filter.ToDateExclusive.HasValue)
        {
            query = query.Where(item => item.ReceptionDate < filter.ToDateExclusive.Value);
        }

        if (filter.CustomerId.HasValue)
        {
            query = query.Where(item => item.CustomerId == filter.CustomerId.Value);
        }

        if (filter.CustomerIds is not null)
        {
            query = query.Where(item => filter.CustomerIds.Contains(item.CustomerId));
        }

        if (filter.CustomerIds is null &&
            !string.IsNullOrWhiteSpace(filter.CustomerSearch))
        {
            var search = filter.CustomerSearch.Trim();
            query = query.Where(item => item.CustomerName.Contains(search));
        }

        if (filter.ReceptionNumber.HasValue)
        {
            query = query.Where(item => item.ReceptionNumber == filter.ReceptionNumber.Value);
        }

        if (filter.ReceptionStatus.HasValue)
        {
            query = query.Where(item => item.ReceptionStatus == filter.ReceptionStatus.Value);
        }

        if (filter.FinancialStatus.HasValue)
        {
            query = query.Where(item =>
                item.ReceptionFinancialStatus == filter.FinancialStatus.Value);
        }

        if (filter.HasAppointment == true)
        {
            query = query.Where(item => item.AppointmentId.HasValue);
        }
        else if (filter.HasAppointment == false)
        {
            query = query.Where(item => !item.AppointmentId.HasValue);
        }

        if (includeServiceFilters &&
            (filter.ServiceId.HasValue ||
             filter.PersonnelId.HasValue ||
             filter.MaterialId.HasValue ||
             filter.ServiceStatus.HasValue))
        {
            query = query.Where(reception =>
                reception.Services.Any(service =>
                    (!filter.ServiceId.HasValue ||
                     service.ServiceId == filter.ServiceId.Value) &&
                    (!filter.PersonnelId.HasValue ||
                     service.PersonnelId == filter.PersonnelId.Value) &&
                    (!filter.ServiceStatus.HasValue ||
                     service.ServiceStatus == filter.ServiceStatus.Value) &&
                    (!filter.MaterialId.HasValue ||
                     service.Materials.Any(material =>
                         material.MaterialId == filter.MaterialId.Value))));
        }

        return query;
    }

    private IQueryable<ReceptionService> FilterServices(
        ReceptionReportFilter filter,
        bool defaultToCompleted)
    {
        var receptionIds = FilterReceptions(filter, includeServiceFilters: false)
            .Where(reception => reception.ReceptionStatus != ReceptionStatus.Cancelled)
            .Select(reception => reception.Id);
        return context.ReceptionServices
            .AsNoTracking()
            .Where(service => receptionIds.Contains(service.ReceptionId))
            .Where(service =>
                service.ServiceStatus != ReceptionServiceStatus.Cancelled &&
                (!filter.ServiceId.HasValue ||
                 service.ServiceId == filter.ServiceId.Value) &&
                (!filter.PersonnelId.HasValue ||
                 service.PersonnelId == filter.PersonnelId.Value) &&
                (!filter.MaterialId.HasValue ||
                 service.Materials.Any(material =>
                     material.MaterialId == filter.MaterialId.Value)) &&
                (filter.ServiceStatus.HasValue
                    ? service.ServiceStatus == filter.ServiceStatus.Value
                    : !defaultToCompleted ||
                      service.ServiceStatus == ReceptionServiceStatus.Completed));
    }

    private sealed class TrendAccumulator(int receptionCount)
    {
        public int ReceptionCount { get; } = receptionCount;
        public decimal ServiceSubtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ServiceFinalAmount { get; set; }
        public decimal PaidAmount { get; set; }
    }
}


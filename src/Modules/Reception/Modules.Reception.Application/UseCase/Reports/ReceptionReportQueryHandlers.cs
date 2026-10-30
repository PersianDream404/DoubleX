using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.Extensions;
using Framwork.PagedList;
using Modules.Customer.Application.Contract.DTOs.Customers.Get;
using Modules.Customer.Application.Contract.UseCase.Customers.Queries.ReportLookup;
using Modules.Inventory.Application.Contract.DTOs.Materials;
using Modules.Inventory.Application.Contract.UseCase;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;
using Modules.Reception.Application.Contract.DTOs.Reports;
using Modules.Reception.Application.Contract.UseCase.Reports.Queries;
using Modules.Reception.Domain.Interfaces;
using Modules.Reception.Domain.ReadModels;

namespace Modules.Reception.Application.UseCase.Reports;

public sealed class GetReceptionReportSummaryQueryHandler(
    IReceptionReportQueryRepository repository)
    : IQueryHandler<GetReceptionReportSummaryQuery, ReceptionReportSummaryDto>
{
    public async Task<Result<ReceptionReportSummaryDto>> Handle(
        GetReceptionReportSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var filter = ReceptionReportQueryMapper.ToFilter(
            request.SalonId!.Value,
            request.FromDate,
            request.ToDateExclusive);
        var result = await repository.GetSummaryAsync(filter, cancellationToken);
        return Result.Success(ReceptionReportQueryMapper.ToDto(result));
    }
}

public sealed class GetReceptionReportReceptionsQueryHandler(
    IReceptionReportQueryRepository repository,
    IQueryBus queryBus)
    : IQueryHandler<
        GetReceptionReportReceptionsQuery,
        ReceptionReportPagedDto<ReceptionReportReceptionDto, ReceptionReportTotalsDto>>
{
    public async Task<Result<ReceptionReportPagedDto<ReceptionReportReceptionDto, ReceptionReportTotalsDto>>> Handle(
        GetReceptionReportReceptionsQuery query,
        CancellationToken cancellationToken)
    {
        var filter = ReceptionReportQueryMapper.ToFilter(query.Request);
        if (!string.IsNullOrWhiteSpace(filter.CustomerSearch))
        {
            var matches = await queryBus.Send<GetCustomerIdsBySearchQuery, IReadOnlyList<long>>(
                new GetCustomerIdsBySearchQuery(filter.SalonId, filter.CustomerSearch),
                cancellationToken);
            if (!matches.IsSuccess)
            {
                return Result.Error(matches.GetErrorMessage());
            }

            filter = filter with { CustomerIds = matches.Value, CustomerSearch = null };
        }

        var result = await repository.GetReceptionsAsync(filter, cancellationToken);
        var customerLookups = await ReceptionReportQueryMapper.GetCustomerLookupsAsync(
            queryBus,
            filter.SalonId,
            result.Page.List.Select(item => item.CustomerId),
            cancellationToken);
        if (!customerLookups.IsSuccess)
        {
            return Result.Error(customerLookups.GetErrorMessage());
        }

        return Result.Success(new ReceptionReportPagedDto<ReceptionReportReceptionDto, ReceptionReportTotalsDto>(
            ReceptionReportQueryMapper.ToPage(result.Page, item =>
                new ReceptionReportReceptionDto(
                    item.Id,
                    item.ReceptionNumber,
                    item.CustomerId,
                    item.CustomerName,
                    customerLookups.Value.GetValueOrDefault(item.CustomerId)?.MobilePhone,
                    item.ReceptionistId,
                    item.ReceptionDate,
                    item.Status,
                    item.FinancialStatus,
                    item.ServiceCount,
                    item.ServiceSubtotal,
                    item.DiscountAmount,
                    item.ServiceFinalAmount,
                    item.PaidAmount,
                    item.RemainingAmount)),
            ReceptionReportQueryMapper.ToDto(result.Totals)));
    }
}

public sealed class GetReceptionReportPersonnelQueryHandler(
    IReceptionReportQueryRepository repository,
    IQueryBus queryBus)
    : IQueryHandler<
        GetReceptionReportPersonnelQuery,
        ReceptionReportPagedDto<ReceptionReportPersonnelDto, ReceptionReportTotalsDto>>
{
    public async Task<Result<ReceptionReportPagedDto<ReceptionReportPersonnelDto, ReceptionReportTotalsDto>>> Handle(
        GetReceptionReportPersonnelQuery query,
        CancellationToken cancellationToken)
    {
        var filter = ReceptionReportQueryMapper.ToFilter(query.Request);
        var resolvedFilter = await ReceptionReportQueryMapper.ResolveCustomerSearchAsync(
            queryBus,
            filter,
            cancellationToken);
        if (!resolvedFilter.IsSuccess)
        {
            return Result.Error(resolvedFilter.GetErrorMessage());
        }

        filter = resolvedFilter.Value;
        var result = await repository.GetPersonnelAsync(filter, cancellationToken);
        var personnelLookups = await ReceptionReportQueryMapper.GetPersonnelLookupsAsync(
            queryBus,
            filter.SalonId,
            result.Page.List.Select(item => item.PersonnelId),
            cancellationToken);
        if (!personnelLookups.IsSuccess)
        {
            return Result.Error(personnelLookups.GetErrorMessage());
        }

        return Result.Success(new ReceptionReportPagedDto<ReceptionReportPersonnelDto, ReceptionReportTotalsDto>(
            ReceptionReportQueryMapper.ToPage(result.Page, item =>
                new ReceptionReportPersonnelDto(
                    item.PersonnelId,
                    personnelLookups.Value.GetValueOrDefault(item.PersonnelId)?.FullName,
                    item.CompletedServiceCount,
                    item.ReceptionCount,
                    item.ServiceSubtotal,
                    item.DiscountAmount,
                    item.ServiceFinalAmount)),
            ReceptionReportQueryMapper.ToDto(result.Totals)));
    }
}

public sealed class GetReceptionReportServicesQueryHandler(
    IReceptionReportQueryRepository repository,
    IQueryBus queryBus)
    : IQueryHandler<
        GetReceptionReportServicesQuery,
        ReceptionReportPagedDto<ReceptionReportServiceDto, ReceptionReportTotalsDto>>
{
    public async Task<Result<ReceptionReportPagedDto<ReceptionReportServiceDto, ReceptionReportTotalsDto>>> Handle(
        GetReceptionReportServicesQuery query,
        CancellationToken cancellationToken)
    {
        var filter = ReceptionReportQueryMapper.ToFilter(query.Request);
        var resolvedFilter = await ReceptionReportQueryMapper.ResolveCustomerSearchAsync(
            queryBus,
            filter,
            cancellationToken);
        if (!resolvedFilter.IsSuccess)
        {
            return Result.Error(resolvedFilter.GetErrorMessage());
        }

        var result = await repository.GetServicesAsync(resolvedFilter.Value, cancellationToken);
        return Result.Success(new ReceptionReportPagedDto<ReceptionReportServiceDto, ReceptionReportTotalsDto>(
            ReceptionReportQueryMapper.ToPage(result.Page, item =>
                new ReceptionReportServiceDto(
                    item.ServiceId,
                    item.ServiceName,
                    item.CompletedServiceCount,
                    item.ReceptionCount,
                    item.ServiceSubtotal,
                    item.DiscountAmount,
                    item.ServiceFinalAmount,
                    item.AverageFinalAmount)),
            ReceptionReportQueryMapper.ToDto(result.Totals)));
    }
}

public sealed class GetReceptionReportDebtsQueryHandler(
    IReceptionReportQueryRepository repository,
    IQueryBus queryBus)
    : IQueryHandler<
        GetReceptionReportDebtsQuery,
        ReceptionReportPagedDto<ReceptionReportDebtDto, ReceptionReportDebtTotalsDto>>
{
    public async Task<Result<ReceptionReportPagedDto<ReceptionReportDebtDto, ReceptionReportDebtTotalsDto>>> Handle(
        GetReceptionReportDebtsQuery query,
        CancellationToken cancellationToken)
    {
        var filter = ReceptionReportQueryMapper.ToFilter(query.Request);
        if (!string.IsNullOrWhiteSpace(filter.CustomerSearch))
        {
            var matches = await queryBus.Send<GetCustomerIdsBySearchQuery, IReadOnlyList<long>>(
                new GetCustomerIdsBySearchQuery(filter.SalonId, filter.CustomerSearch),
                cancellationToken);
            if (!matches.IsSuccess)
            {
                return Result.Error(matches.GetErrorMessage());
            }

            filter = filter with { CustomerIds = matches.Value, CustomerSearch = null };
        }

        var result = await repository.GetDebtsAsync(filter, cancellationToken);
        var customerLookups = await ReceptionReportQueryMapper.GetCustomerLookupsAsync(
            queryBus,
            filter.SalonId,
            result.Page.List.Select(item => item.CustomerId),
            cancellationToken);
        if (!customerLookups.IsSuccess)
        {
            return Result.Error(customerLookups.GetErrorMessage());
        }

        return Result.Success(new ReceptionReportPagedDto<ReceptionReportDebtDto, ReceptionReportDebtTotalsDto>(
            ReceptionReportQueryMapper.ToPage(result.Page, item =>
                new ReceptionReportDebtDto(
                    item.ReceptionId,
                    item.ReceptionNumber,
                    item.CustomerId,
                    item.CustomerName,
                    customerLookups.Value.GetValueOrDefault(item.CustomerId)?.MobilePhone,
                    item.ReceptionDate,
                    item.Status,
                    item.TotalAmount,
                    item.PaidAmount,
                    item.RemainingAmount)),
            new ReceptionReportDebtTotalsDto(
                result.Totals.ReceptionCount,
                result.Totals.TotalAmount,
                result.Totals.PaidAmount,
                result.Totals.RemainingAmount)));
    }
}

public sealed class GetReceptionReportDiscountsQueryHandler(
    IReceptionReportQueryRepository repository,
    IQueryBus queryBus)
    : IQueryHandler<
        GetReceptionReportDiscountsQuery,
        ReceptionReportPagedDto<ReceptionReportDiscountDto, ReceptionReportDiscountTotalsDto>>
{
    public async Task<Result<ReceptionReportPagedDto<ReceptionReportDiscountDto, ReceptionReportDiscountTotalsDto>>> Handle(
        GetReceptionReportDiscountsQuery query,
        CancellationToken cancellationToken)
    {
        var filter = ReceptionReportQueryMapper.ToFilter(query.Request);
        if (!string.IsNullOrWhiteSpace(filter.CustomerSearch))
        {
            var matches = await queryBus.Send<GetCustomerIdsBySearchQuery, IReadOnlyList<long>>(
                new GetCustomerIdsBySearchQuery(filter.SalonId, filter.CustomerSearch),
                cancellationToken);
            if (!matches.IsSuccess)
            {
                return Result.Error(matches.GetErrorMessage());
            }

            filter = filter with { CustomerIds = matches.Value, CustomerSearch = null };
        }

        var result = await repository.GetDiscountsAsync(filter, cancellationToken);
        var customerLookups = await ReceptionReportQueryMapper.GetCustomerLookupsAsync(
            queryBus,
            filter.SalonId,
            result.Page.List.Select(item => item.CustomerId),
            cancellationToken);
        if (!customerLookups.IsSuccess)
        {
            return Result.Error(customerLookups.GetErrorMessage());
        }

        return Result.Success(new ReceptionReportPagedDto<ReceptionReportDiscountDto, ReceptionReportDiscountTotalsDto>(
            ReceptionReportQueryMapper.ToPage(result.Page, item =>
                new ReceptionReportDiscountDto(
                    item.ReceptionId,
                    item.ReceptionNumber,
                    item.CustomerId,
                    item.CustomerName,
                    customerLookups.Value.GetValueOrDefault(item.CustomerId)?.MobilePhone,
                    item.ReceptionDate,
                    item.ServiceId,
                    item.ServiceName,
                    item.ServiceSubtotal,
                    item.DiscountAmount,
                    item.ServiceFinalAmount)),
            new ReceptionReportDiscountTotalsDto(
                result.Totals.ServiceCount,
                result.Totals.ServiceSubtotal,
                result.Totals.DiscountAmount,
                result.Totals.ServiceFinalAmount)));
    }
}

public sealed class GetReceptionReportMaterialsQueryHandler(
    IReceptionReportQueryRepository repository,
    IQueryBus queryBus)
    : IQueryHandler<
        GetReceptionReportMaterialsQuery,
        ReceptionReportPagedDto<ReceptionReportMaterialDto, ReceptionReportMaterialTotalsDto>>
{
    public async Task<Result<ReceptionReportPagedDto<ReceptionReportMaterialDto, ReceptionReportMaterialTotalsDto>>> Handle(
        GetReceptionReportMaterialsQuery query,
        CancellationToken cancellationToken)
    {
        var filter = ReceptionReportQueryMapper.ToFilter(query.Request);
        var resolvedFilter = await ReceptionReportQueryMapper.ResolveCustomerSearchAsync(
            queryBus,
            filter,
            cancellationToken);
        if (!resolvedFilter.IsSuccess)
        {
            return Result.Error(resolvedFilter.GetErrorMessage());
        }

        filter = resolvedFilter.Value;
        var result = await repository.GetMaterialsAsync(filter, cancellationToken);
        var materialLookups = await ReceptionReportQueryMapper.GetMaterialLookupsAsync(
            queryBus,
            result.Page.List.Select(item => item.MaterialId),
            cancellationToken);
        if (!materialLookups.IsSuccess)
        {
            return Result.Error(materialLookups.GetErrorMessage());
        }

        return Result.Success(new ReceptionReportPagedDto<ReceptionReportMaterialDto, ReceptionReportMaterialTotalsDto>(
            ReceptionReportQueryMapper.ToPage(result.Page, item =>
                new ReceptionReportMaterialDto(
                    item.MaterialId,
                    item.ServiceId,
                    item.ServiceName,
                    materialLookups.Value.GetValueOrDefault(item.MaterialId)?.Name,
                    materialLookups.Value.GetValueOrDefault(item.MaterialId)?.BaseUnitName,
                    materialLookups.Value.GetValueOrDefault(item.MaterialId)?.BaseUnitSymbol,
                    item.UsageCount,
                    item.Quantity,
                    item.TotalCost)),
            new ReceptionReportMaterialTotalsDto(
                result.Totals.UsageCount,
                result.Totals.Quantity,
                result.Totals.TotalCost)));
    }
}

public sealed class GetReceptionReportSummaryQueryValidator
    : AbstractValidator<GetReceptionReportSummaryQuery>
{
    public GetReceptionReportSummaryQueryValidator()
    {
        RuleFor(query => query.Request).NotNull();
        When(query => query.Request is not null, () =>
        {
            RuleFor(query => query.Request.SalonId).NotNull().GreaterThan(0);
            RuleFor(query => query.Request)
                .Must(request =>
                    !request.FromDate.HasValue ||
                    !request.ToDateExclusive.HasValue ||
                    request.FromDate.Value < request.ToDateExclusive.Value)
                .WithMessage("پایان بازه باید بعد از شروع بازه باشد.");
        });
    }
}

public abstract class ReceptionReportPagedRequestValidator<TRequest>
    : AbstractValidator<TRequest>
    where TRequest : ReceptionReportPagedRequestDto
{
    protected ReceptionReportPagedRequestValidator()
    {
        RuleFor(request => request.SalonId).NotNull().GreaterThan(0);
        RuleFor(request => request.FromDate)
            .LessThan(request => request.ToDateExclusive)
            .When(request => request.FromDate.HasValue && request.ToDateExclusive.HasValue)
            .WithMessage("پایان بازه باید بعد از شروع بازه باشد.");
        RuleFor(request => request.CustomerSearch).MaximumLength(250);
        RuleFor(request => request.CustomerId).GreaterThan(0).When(request =>
            request.CustomerId.HasValue);
        RuleFor(request => request.ReceptionNumber).GreaterThan(0).When(request =>
            request.ReceptionNumber.HasValue);
        RuleFor(request => request.PersonnelId).GreaterThan(0).When(request =>
            request.PersonnelId.HasValue);
        RuleFor(request => request.ServiceId).GreaterThan(0).When(request =>
            request.ServiceId.HasValue);
        RuleFor(request => request.MaterialId).GreaterThan(0).When(request =>
            request.MaterialId.HasValue);
        RuleFor(request => request.MinimumRemainingAmount).GreaterThanOrEqualTo(0m)
            .When(request => request.MinimumRemainingAmount.HasValue);
        RuleFor(request => request.MaximumRemainingAmount).GreaterThanOrEqualTo(0m)
            .When(request => request.MaximumRemainingAmount.HasValue);
        RuleFor(request => request)
            .Must(request =>
                !request.MinimumRemainingAmount.HasValue ||
                !request.MaximumRemainingAmount.HasValue ||
                request.MinimumRemainingAmount <= request.MaximumRemainingAmount)
            .WithMessage("حداقل مانده نباید از حداکثر مانده بیشتر باشد.");
        RuleFor(request => request.PageNumber).GreaterThan(0);
        RuleFor(request => request.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class GetReceptionReportReceptionsQueryValidator
    : ReceptionReportPagedRequestValidator<GetReceptionReportReceptionsRequestDto> { }

public sealed class GetReceptionReportPersonnelQueryValidator
    : ReceptionReportPagedRequestValidator<GetReceptionReportPersonnelRequestDto> { }

public sealed class GetReceptionReportServicesQueryValidator
    : ReceptionReportPagedRequestValidator<GetReceptionReportServicesRequestDto> { }

public sealed class GetReceptionReportDebtsQueryValidator
    : ReceptionReportPagedRequestValidator<GetReceptionReportDebtsRequestDto> { }

public sealed class GetReceptionReportDiscountsQueryValidator
    : ReceptionReportPagedRequestValidator<GetReceptionReportDiscountsRequestDto> { }

public sealed class GetReceptionReportMaterialsQueryValidator
    : ReceptionReportPagedRequestValidator<GetReceptionReportMaterialsRequestDto> { }

internal static class ReceptionReportQueryMapper
{
    public static async Task<Result<ReceptionReportFilter>> ResolveCustomerSearchAsync(
        IQueryBus queryBus,
        ReceptionReportFilter filter,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filter.CustomerSearch))
        {
            return Result.Success(filter);
        }

        var matches = await queryBus.Send<GetCustomerIdsBySearchQuery, IReadOnlyList<long>>(
            new GetCustomerIdsBySearchQuery(filter.SalonId, filter.CustomerSearch),
            cancellationToken);
        return matches.IsSuccess
            ? Result.Success(filter with
            {
                CustomerIds = matches.Value,
                CustomerSearch = null
            })
            : Result.Error(matches.GetErrorMessage());
    }

    public static async Task<Result<IReadOnlyDictionary<long, CustomerReceptionLookupDto>>> GetCustomerLookupsAsync(
        IQueryBus queryBus,
        long salonId,
        IEnumerable<long> customerIds,
        CancellationToken cancellationToken)
    {
        var ids = customerIds.Distinct().ToArray();
        if (ids.Length == 0)
        {
            return Result.Success<IReadOnlyDictionary<long, CustomerReceptionLookupDto>>(
                new Dictionary<long, CustomerReceptionLookupDto>());
        }

        var result = await queryBus.Send<
            GetCustomerReceptionLookupsQuery,
            IReadOnlyList<CustomerReceptionLookupDto>>(
            new GetCustomerReceptionLookupsQuery(salonId, ids),
            cancellationToken);
        return result.IsSuccess
            ? Result.Success<IReadOnlyDictionary<long, CustomerReceptionLookupDto>>(
                result.Value.ToDictionary(item => item.Id))
            : Result.Error(result.GetErrorMessage());
    }

    public static async Task<Result<IReadOnlyDictionary<long, PersonnelReceptionLookupDto>>> GetPersonnelLookupsAsync(
        IQueryBus queryBus,
        long salonId,
        IEnumerable<long> personnelIds,
        CancellationToken cancellationToken)
    {
        var ids = personnelIds.Distinct().ToArray();
        if (ids.Length == 0)
        {
            return Result.Success<IReadOnlyDictionary<long, PersonnelReceptionLookupDto>>(
                new Dictionary<long, PersonnelReceptionLookupDto>());
        }

        var result = await queryBus.Send<
            GetPersonnelReceptionLookupsQuery,
            IReadOnlyList<PersonnelReceptionLookupDto>>(
            new GetPersonnelReceptionLookupsQuery(salonId, ids),
            cancellationToken);
        return result.IsSuccess
            ? Result.Success<IReadOnlyDictionary<long, PersonnelReceptionLookupDto>>(
                result.Value.ToDictionary(item => item.Id))
            : Result.Error(result.GetErrorMessage());
    }

    public static async Task<Result<IReadOnlyDictionary<long, MaterialReceptionLookupDto>>> GetMaterialLookupsAsync(
        IQueryBus queryBus,
        IEnumerable<long> materialIds,
        CancellationToken cancellationToken)
    {
        var ids = materialIds.Distinct().ToArray();
        if (ids.Length == 0)
        {
            return Result.Success<IReadOnlyDictionary<long, MaterialReceptionLookupDto>>(
                new Dictionary<long, MaterialReceptionLookupDto>());
        }

        var result = await queryBus.Send<
            GetMaterialReceptionLookupsQuery,
            IReadOnlyList<MaterialReceptionLookupDto>>(
            new GetMaterialReceptionLookupsQuery(ids),
            cancellationToken);
        return result.IsSuccess
            ? Result.Success<IReadOnlyDictionary<long, MaterialReceptionLookupDto>>(
                result.Value.ToDictionary(item => item.Id))
            : Result.Error(result.GetErrorMessage());
    }

    public static ReceptionReportFilter ToFilter(ReceptionReportPagedRequestDto request) =>
        new(
            request.SalonId!.Value,
            request.FromDate,
            request.ToDateExclusive,
            request.CustomerSearch,
            null,
            request.CustomerId,
            request.ReceptionNumber,
            request.PersonnelId,
            request.ServiceId,
            request.MaterialId,
            request.ReceptionStatus,
            request.FinancialStatus,
            request.ServiceStatus,
            request.HasAppointment,
            request.MinimumRemainingAmount,
            request.MaximumRemainingAmount,
            request.PageNumber ?? 1,
            request.PageSize ?? 50,
            request.OldestFirst ?? false);

    public static ReceptionReportFilter ToFilter(
        long salonId,
        DateTimeOffset? fromDate,
        DateTimeOffset? toDateExclusive) =>
        new(
            salonId,
            fromDate,
            toDateExclusive,
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

    public static ReceptionReportSummaryDto ToDto(ReceptionReportSummaryReadModel item) =>
        new(
            item.ReceptionCount,
            item.CompletedReceptionCount,
            item.InProgressReceptionCount,
            item.CancelledReceptionCount,
            item.ServiceSubtotal,
            item.DiscountAmount,
            item.ServiceFinalAmount,
            item.PaidAmount,
            item.RemainingAmount,
            item.MaterialChargeAmount,
            item.Trend.Select(trend => new ReceptionReportTrendDto(
                trend.Date,
                trend.ReceptionCount,
                trend.ServiceSubtotal,
                trend.DiscountAmount,
                trend.ServiceFinalAmount,
                trend.PaidAmount)).ToArray(),
            item.PaymentMethods.Select(payment => new ReceptionReportPaymentMethodDto(
                payment.PaymentMethod,
                payment.Amount)).ToArray());

    public static ReceptionReportTotalsDto ToDto(ReceptionReportTotalsReadModel item) =>
        new(
            item.ReceptionCount,
            item.ServiceSubtotal,
            item.DiscountAmount,
            item.ServiceFinalAmount,
            item.PaidAmount,
            item.RemainingAmount);

    public static PagedList<TDestination> ToPage<TSource, TDestination>(
        PagedList<TSource> source,
        Func<TSource, TDestination> map) =>
        new()
        {
            List = source.List.Select(map).ToList(),
            Pagination = source.Pagination
        };
}

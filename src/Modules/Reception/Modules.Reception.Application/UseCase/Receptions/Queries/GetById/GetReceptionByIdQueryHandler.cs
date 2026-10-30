using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.Extensions;
using Modules.Reception.Application.UseCase.Reports;
using Modules.Reception.Application.Contract.DTOs.Receptions.Get;
using Modules.Reception.Application.Contract.UseCase.Receptions.Queries.GetById;
using Modules.Reception.Domain.Interfaces;

namespace Modules.Reception.Application.UseCase.Receptions.Queries.GetById;

public sealed class GetReceptionByIdQueryHandler(
    IReceptionQueryRepository repository,
    IQueryBus queryBus)
    : IQueryHandler<GetReceptionByIdQuery, ReceptionDetailsDto>
{
    public async Task<Result<ReceptionDetailsDto>> Handle(
        GetReceptionByIdQuery query,
        CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(query.Id, query.SalonId, cancellationToken);
        if (item is null)
        {
            return Result.NotFound("پذیرش موردنظر پیدا نشد.");
        }

        var customerLookups = await ReceptionReportQueryMapper.GetCustomerLookupsAsync(
            queryBus,
            item.SalonId,
            [item.CustomerId],
            cancellationToken);
        if (!customerLookups.IsSuccess)
        {
            return Result.Error(customerLookups.GetErrorMessage());
        }

        var personnelLookups = await ReceptionReportQueryMapper.GetPersonnelLookupsAsync(
            queryBus,
            item.SalonId,
            item.Services
                .Where(service => service.PersonnelId.HasValue)
                .Select(service => service.PersonnelId!.Value),
            cancellationToken);
        if (!personnelLookups.IsSuccess)
        {
            return Result.Error(personnelLookups.GetErrorMessage());
        }

        var materialLookups = await ReceptionReportQueryMapper.GetMaterialLookupsAsync(
            queryBus,
            item.Services.SelectMany(service => service.Materials)
                .Select(material => material.MaterialId),
            cancellationToken);
        if (!materialLookups.IsSuccess)
        {
            return Result.Error(materialLookups.GetErrorMessage());
        }

        return Result.Success(new ReceptionDetailsDto(
            item.Id,
            item.SalonId,
            item.ReceptionNumber,
            item.CustomerId,
            item.CustomerName,
            customerLookups.Value.GetValueOrDefault(item.CustomerId)?.MobilePhone,
            customerLookups.Value.GetValueOrDefault(item.CustomerId)?.Status,
            item.ReceptionistId,
            item.ServiceTariffId,
            item.AppointmentId,
            item.ReceptionDate,
            item.Status,
            item.SubTotal,
            item.TotalDiscountAmount,
            item.TotalAmount,
            item.PaidAmount,
            item.RemainingAmount,
            item.FinancialStatus,
            item.Notes,
            item.Services.Select(service => new ReceptionServiceDto(
                service.Id,
                service.ServiceId,
                service.ServiceTariffId,
                service.PersonnelId,
                service.PersonnelId.HasValue
                    ? personnelLookups.Value.GetValueOrDefault(service.PersonnelId.Value)?.FullName
                    : null,
                service.ServiceName,
                service.Quantity,
                service.UnitPrice,
                service.DiscountAmount,
                service.FinalAmount,
                service.Status,
                service.Materials.Select(material => new ReceptionServiceMaterialDto(
                    material.Id,
                    material.MaterialId,
                    materialLookups.Value.GetValueOrDefault(material.MaterialId)?.Name,
                    materialLookups.Value.GetValueOrDefault(material.MaterialId)?.BaseUnitName,
                    materialLookups.Value.GetValueOrDefault(material.MaterialId)?.BaseUnitSymbol,
                    material.Quantity,
                    material.UnitCost,
                    material.TotalCost,
                    material.Description,
                    material.MovementRegistered,
                    material.MovementReversed)).ToList())).ToList(),
            item.Payments.Select(payment => new ReceptionPaymentDto(
                payment.Id,
                payment.Amount,
                payment.PaymentMethod,
                payment.PaidAt,
                payment.Status,
                payment.TransactionReference,
                payment.Description,
                payment.CashRegisterId)).ToList()));
    }
}

public sealed class GetReceptionByIdQueryValidator : AbstractValidator<GetReceptionByIdQuery>
{
    public GetReceptionByIdQueryValidator()
    {
        RuleFor(query => query.Id).GreaterThan(0);
        RuleFor(query => query.SalonId).GreaterThan(0);
    }
}

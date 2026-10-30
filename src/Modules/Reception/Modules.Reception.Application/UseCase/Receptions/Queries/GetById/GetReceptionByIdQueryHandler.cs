using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Reception.Application.Contract.DTOs.Receptions.Get;
using Modules.Reception.Application.Contract.UseCase.Receptions.Queries.GetById;
using Modules.Reception.Domain.Interfaces;

namespace Modules.Reception.Application.UseCase.Receptions.Queries.GetById;

public sealed class GetReceptionByIdQueryHandler(IReceptionQueryRepository repository)
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

        return Result.Success(new ReceptionDetailsDto(
            item.Id,
            item.SalonId,
            item.ReceptionNumber,
            item.CustomerId,
            item.CustomerName,
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
                service.ServiceName,
                service.Quantity,
                service.UnitPrice,
                service.DiscountAmount,
                service.FinalAmount,
                service.Status,
                service.Materials.Select(material => new ReceptionServiceMaterialDto(
                    material.Id,
                    material.MaterialId,
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

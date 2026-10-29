using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Reception.Application.Contract.DTOs.Receptions.GetAll;
using Modules.Reception.Application.Contract.UseCase.Receptions.Queries.GetAll;
using Modules.Reception.Domain.Interfaces;

namespace Modules.Reception.Application.UseCase.Receptions.Queries.GetAll;

public sealed class GetReceptionsQueryHandler(IReceptionQueryRepository repository)
    : IQueryHandler<GetReceptionsQuery, PagedList<ReceptionListItemDto>>
{
    public async Task<Result<PagedList<ReceptionListItemDto>>> Handle(
        GetReceptionsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var page = await repository.GetAllAsync(
            request.SalonId!.Value,
            request.CustomerId,
            request.UnpaidOnly == true,
            request.PageNumber ?? 1,
            request.PageSize ?? 50,
            cancellationToken);
        return Result.Success(new PagedList<ReceptionListItemDto>
        {
            List = page.List.Select(item => new ReceptionListItemDto(
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
                item.TotalAmount,
                item.PaidAmount,
                item.RemainingAmount,
                item.FinancialStatus)).ToList(),
            Pagination = page.Pagination
        });
    }
}

public sealed class GetReceptionsQueryValidator : AbstractValidator<GetReceptionsQuery>
{
    public GetReceptionsQueryValidator()
    {
        RuleFor(query => query.Request).NotNull();
        When(query => query.Request is not null, () =>
        {
            RuleFor(query => query.Request.SalonId)
                .NotNull()
                .GreaterThan(0);
            RuleFor(query => query.Request.CustomerId).GreaterThan(0).When(query =>
                query.Request.CustomerId.HasValue);
            RuleFor(query => query.Request.PageNumber).GreaterThan(0);
            RuleFor(query => query.Request.PageSize).InclusiveBetween(1, 100);
        });
    }
}

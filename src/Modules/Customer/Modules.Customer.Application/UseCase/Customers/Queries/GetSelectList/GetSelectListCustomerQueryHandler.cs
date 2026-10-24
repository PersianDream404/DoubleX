using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Customer.Application.Contract.DTOs.Customers.GetAll;
using Modules.Customer.Application.Contract.Interfaces.Customers;
using Modules.Customer.Application.Contract.UseCase.Customers.Queries.GetSelectList;

namespace Modules.Customer.Application.UseCase.Customers.Queries.GetSelectList;

public sealed class GetSelectListCustomerQueryHandler(ICustomerQueryRepository repository)
    : IQueryHandler<GetSelectListCustomerQuery, PagedList<GetSelectListCustomerResponseDto>>
{
    public async Task<Result<PagedList<GetSelectListCustomerResponseDto>>> Handle(
        GetSelectListCustomerQuery query,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetSelectListProjectedAsync(
            query.Request,
            cancellationToken);
        return Result.Success(result);
    }
}

public sealed class GetSelectListCustomerQueryValidator
    : AbstractValidator<GetSelectListCustomerQuery>
{
    public GetSelectListCustomerQueryValidator()
    {
        RuleFor(query => query.Request).NotNull();
        RuleFor(query => query.Request.SalonId).GreaterThan(0);
        RuleFor(query => query.Request.Q).MaximumLength(250);
    }
}

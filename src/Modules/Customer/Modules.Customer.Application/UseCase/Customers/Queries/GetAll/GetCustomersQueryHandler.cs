using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Customer.Application.Contract.DTOs.Customers.GetAll;
using Modules.Customer.Application.Contract.Interfaces.Customers;
using Modules.Customer.Application.Contract.UseCase.Customers.Queries.GetAll;

namespace Modules.Customer.Application.UseCase.Customers.Queries.GetAll;

public sealed class GetCustomersQueryHandler(ICustomerQueryRepository repository)
    : IQueryHandler<GetCustomersQuery, PagedList<CustomerListItemDto>>
{
    public async Task<Result<PagedList<CustomerListItemDto>>> Handle(
        GetCustomersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetAllProjectedAsync(query.Request, cancellationToken);
        return Result.Success(result);
    }
}

public sealed class GetCustomersQueryValidator : AbstractValidator<GetCustomersQuery>
{
    public GetCustomersQueryValidator()
    {
        RuleFor(query => query.Request).NotNull();
        RuleFor(query => query.Request.SalonId).GreaterThan(0);
        RuleFor(query => query.Request.Q).MaximumLength(250);
    }
}

using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Customer.Application.Contract.DTOs.Customers.Get;
using Modules.Customer.Application.Contract.Interfaces.Customers;
using Modules.Customer.Application.Contract.UseCase.Customers.Queries.Get;

namespace Modules.Customer.Application.UseCase.Customers.Queries.Get;

public sealed class GetCustomerByIdQueryHandler(ICustomerQueryRepository repository)
    : IQueryHandler<GetCustomerByIdQuery, CustomerDetailsDto>
{
    public async Task<Result<CustomerDetailsDto>> Handle(
        GetCustomerByIdQuery query,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetByIdProjectedAsync(query.Id, cancellationToken);
        return result is null
            ? Result.NotFound("مشتری موردنظر پیدا نشد.")
            : Result.Success(result);
    }
}

public sealed class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdQueryValidator()
    {
        RuleFor(query => query.Id).GreaterThan(0);
    }
}

using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Customer.Application.Contract.DTOs.Customers.Get;
using Modules.Customer.Application.Contract.Interfaces.Customers;
using Modules.Customer.Application.Contract.UseCase.Customers.Queries.ReportLookup;

namespace Modules.Customer.Application.UseCase.Customers.Queries.ReportLookup;

public sealed class GetCustomerReceptionLookupsQueryHandler(
    ICustomerQueryRepository repository)
    : IQueryHandler<
        GetCustomerReceptionLookupsQuery,
        IReadOnlyList<CustomerReceptionLookupDto>>
{
    public async Task<Result<IReadOnlyList<CustomerReceptionLookupDto>>> Handle(
        GetCustomerReceptionLookupsQuery query,
        CancellationToken cancellationToken) =>
        Result.Success(await repository.GetReceptionLookupsAsync(
            query.SalonId,
            query.CustomerIds,
            cancellationToken));
}

public sealed class GetCustomerIdsBySearchQueryHandler(ICustomerQueryRepository repository)
    : IQueryHandler<GetCustomerIdsBySearchQuery, IReadOnlyList<long>>
{
    public async Task<Result<IReadOnlyList<long>>> Handle(
        GetCustomerIdsBySearchQuery query,
        CancellationToken cancellationToken) =>
        Result.Success(await repository.GetIdsBySearchAsync(
            query.SalonId,
            query.Search,
            cancellationToken));
}

public sealed class GetCustomerReceptionLookupsQueryValidator
    : AbstractValidator<GetCustomerReceptionLookupsQuery>
{
    public GetCustomerReceptionLookupsQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.CustomerIds)
            .NotNull()
            .Must(ids => ids.Count <= 501)
            .WithMessage("تعداد شناسه‌های مشتری بیش از حد مجاز است.");
        RuleForEach(query => query.CustomerIds).GreaterThan(0);
    }
}

public sealed class GetCustomerIdsBySearchQueryValidator
    : AbstractValidator<GetCustomerIdsBySearchQuery>
{
    public GetCustomerIdsBySearchQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.Search).NotEmpty().MaximumLength(250);
    }
}

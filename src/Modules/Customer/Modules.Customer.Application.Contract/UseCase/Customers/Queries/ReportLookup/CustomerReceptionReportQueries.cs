using Framwork.Bus.Query;
using Modules.Customer.Application.Contract.DTOs.Customers.Get;

namespace Modules.Customer.Application.Contract.UseCase.Customers.Queries.ReportLookup;

public sealed record GetCustomerReceptionLookupsQuery(
    long SalonId,
    IReadOnlyCollection<long> CustomerIds)
    : IQuery<IReadOnlyList<CustomerReceptionLookupDto>>;

public sealed record GetCustomerIdsBySearchQuery(long SalonId, string Search)
    : IQuery<IReadOnlyList<long>>;

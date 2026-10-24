using Framwork.Bus.Query;
using Modules.Customer.Application.Contract.DTOs.Customers.Get;

namespace Modules.Customer.Application.Contract.UseCase.Customers.Queries.Get;

public record GetCustomerByIdQuery(long Id) : IQuery<CustomerDetailsDto>;

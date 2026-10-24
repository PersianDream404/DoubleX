using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Customer.Application.Contract.DTOs.Customers.GetAll;

namespace Modules.Customer.Application.Contract.UseCase.Customers.Queries.GetAll;

public record GetCustomersQuery(GetCustomersRequestDto Request)
    : IQuery<PagedList<CustomerListItemDto>>;

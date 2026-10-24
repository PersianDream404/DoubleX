using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Customer.Application.Contract.DTOs.Customers.GetAll;

namespace Modules.Customer.Application.Contract.UseCase.Customers.Queries.GetSelectList;

public sealed record GetSelectListCustomerQuery(GetSelectListCustomerRequestDto Request)
    : IQuery<PagedList<GetSelectListCustomerResponseDto>>;

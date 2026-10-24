using Framwork.PagedList;
using Modules.Customer.Application.Contract.DTOs.Customers.Get;
using Modules.Customer.Application.Contract.DTOs.Customers.GetAll;
using SharedKernel.Interface.Repositories;
using CustomerEntity = Modules.Customer.Domain.Entities.Customer;

namespace Modules.Customer.Application.Contract.Interfaces.Customers;

public interface ICustomerQueryRepository : IQueryRepository<CustomerEntity>
{
    Task<PagedList<CustomerListItemDto>> GetAllProjectedAsync(
        GetCustomersRequestDto request,
        CancellationToken cancellationToken);

    Task<PagedList<GetSelectListCustomerResponseDto>> GetSelectListProjectedAsync(
        GetSelectListCustomerRequestDto request,
        CancellationToken cancellationToken);

    Task<CustomerDetailsDto?> GetByIdProjectedAsync(
        long id,
        CancellationToken cancellationToken);

    Task<CustomerEntity?> GetEntityByIdAsync(long id, CancellationToken cancellationToken);
    Task<bool> SalonExistsAsync(long salonId, CancellationToken cancellationToken);
}

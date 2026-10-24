using Infrastructure.Repositories;
using Modules.Customer.Domain.Interface;
using Modules.Customer.Persistence.Context;
using CustomerEntity = Modules.Customer.Domain.Entities.Customer;

namespace Modules.Customer.Persistence.Repositories.Customers;

public sealed class CustomerCommandRepository(CustomerWriteDbContext context)
    : CommandRepository<CustomerEntity>(context), ICustomerCommandRepository
{
}

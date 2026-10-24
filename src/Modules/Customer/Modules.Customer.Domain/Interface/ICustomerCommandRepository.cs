using CustomerEntity = Modules.Customer.Domain.Entities.Customer;
using SharedKernel.Interface.Repositories;

namespace Modules.Customer.Domain.Interface;

public interface ICustomerCommandRepository : ICommandRepository<CustomerEntity>
{
}

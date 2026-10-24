using Framwork.Bus.Command;
using Modules.Customer.Application.Contract.DTOs.Customers.Update;

namespace Modules.Customer.Application.Contract.UseCase.Customers.Commands.Update;

public record UpdateCustomerCommand(UpdateCustomerRequestDto Request) : ICommand<bool>;

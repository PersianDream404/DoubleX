using Framwork.Bus.Command;
using Modules.Customer.Application.Contract.DTOs.Customers.Create;

namespace Modules.Customer.Application.Contract.UseCase.Customers.Commands.Create;

public record CreateCustomerCommand(CreateCustomerRequestDto Request) : ICommand<bool>;

using Framwork.Bus.Command;

namespace Modules.Customer.Application.Contract.UseCase.Customers.Commands.Toggle;

public record ToggleCustomerCommand(long Id) : ICommand<bool>;

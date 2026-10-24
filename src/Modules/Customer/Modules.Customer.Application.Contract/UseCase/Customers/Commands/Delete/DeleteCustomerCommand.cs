using Framwork.Bus.Command;

namespace Modules.Customer.Application.Contract.UseCase.Customers.Commands.Delete;

public record DeleteCustomerCommand(long Id) : ICommand<bool>;

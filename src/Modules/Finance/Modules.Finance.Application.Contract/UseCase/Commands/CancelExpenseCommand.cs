using Framwork.Bus.Command;

namespace Modules.Finance.Application.Contract.UseCase.Commands;

public sealed record CancelExpenseCommand(long SalonId, long Id, string Reason)
    : ICommand<bool>;

using Framwork.Bus.Command;

namespace Modules.Finance.Application.Contract.UseCase.Commands;

public sealed record CalculateReceptionEarningsCommand(long SalonId, long ReceptionId)
    : ICommand<int>;

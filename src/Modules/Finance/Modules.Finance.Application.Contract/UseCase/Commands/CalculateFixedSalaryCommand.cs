using Framwork.Bus.Command;

namespace Modules.Finance.Application.Contract.UseCase.Commands;

public sealed record CalculateFixedSalaryCommand(
    long SalonId,
    long PersonnelId,
    DateOnly PeriodStart,
    DateOnly PeriodEnd) : ICommand<long>;

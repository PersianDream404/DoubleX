using Framwork.Bus.Command;
using Modules.Reception.Application.Contract.DTOs.Payments.Register;

namespace Modules.Reception.Application.Contract.UseCase.Payments.Commands.Register;

public sealed record RegisterReceptionPaymentCommand(
    long ReceptionId,
    RegisterReceptionPaymentRequestDto Request) : ICommand<bool>;

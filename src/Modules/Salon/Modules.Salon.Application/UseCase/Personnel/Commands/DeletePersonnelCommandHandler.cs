using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Validation.Resources;
using Modules.Personnel.Application.Common;
using Modules.Personnel.Application.Contract.Interfaces;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Commands;
using Modules.Personnel.Domain.Interface;

namespace Modules.Personnel.Application.UseCase.Personnel.Commands;

public sealed class DeletePersonnelCommandHandler(
    IPersonnelCommandRepository personnelCommandRepository,
    IPersonnelQueryRepository personnelQueryRepository)
    : ICommandHandler<DeletePersonnelCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeletePersonnelCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var personnel = await personnelQueryRepository.GetByIdAsync(command.Id, cancellationToken);
            if (personnel is null)
                return Result.Error(Messages.PersonnelNotFound);

            await personnelCommandRepository.DeleteAsync(command.Id, cancellationToken);
            return Result.Success(true);
        }
        catch (Exception)
        {
            return Result.Error(Messages.ErrorInPersonnel);
        }
    }
}

public sealed class DeletePersonnelCommandValidator : AbstractValidator<DeletePersonnelCommand>
{
    public DeletePersonnelCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage(SharedValidationMessages.Required);
    }
}

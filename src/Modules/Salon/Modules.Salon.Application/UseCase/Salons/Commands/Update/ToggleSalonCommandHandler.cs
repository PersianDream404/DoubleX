using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Validation.Resources;
using Modules.Salon.Application.Contract.UseCase.Salons.Commands;
using Modules.Salon.Domain.Interface;
using Modules.Product.Application.Contract.Interface.Salons;
using SharedKernel.Constants;
using SharedKernel.Helper;

namespace Modules.Product.Application.UseCase.Salons.Commands.Update;

public class ToggleSalonCommandHandler(ISalonCommandRepository colorCommandRepository,ISalonQueryRepository colorQueryRepository)
: ICommandHandler<ToggleSalonCommand, bool>
{
    public async Task<Result<bool>> Handle(
        ToggleSalonCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var color = await colorQueryRepository.GetByIdAsync(command.Id, cancellationToken);
            if (color == null)
                return Result.Error(MessageHelper.Format(AppMessages.NotFound, AppEntity.Salon));

         
            await colorCommandRepository.ToggleAsync(color,cancellationToken);

        }
        catch (Exception)
        {
            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.Salon));
        }

        return true;
    }
}
public class ToggleSalonCommandValidator
    : AbstractValidator<ToggleSalonCommand>
{
    public ToggleSalonCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SharedValidationMessages.Required);

    }
}
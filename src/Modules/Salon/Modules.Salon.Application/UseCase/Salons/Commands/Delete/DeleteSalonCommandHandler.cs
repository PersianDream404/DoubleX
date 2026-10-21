using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Validation.Resources;
using Modules.Product.Application.Contract.Interface.Salons;
using SharedKernel.Constants;
using SharedKernel.Helper;
using Modules.Salon.Domain.Interface;
using Modules.Salon.Application.Contract.UseCase.Salons.Commands;

namespace Modules.Salon.Application.UseCase.Salons.Commands.Delete;

public class DeleteSalonCommandHandler(ISalonCommandRepository colorCommandRepository,ISalonQueryRepository colorQueryRepository)
: ICommandHandler<DeleteSalonCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeleteSalonCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var color = await colorQueryRepository.GetByIdProjectedAsync(command.Id, cancellationToken);
            if (color == null)
                return Result.Error(MessageHelper.Format(AppMessages.NotFound, AppEntity.Salon));

            await colorCommandRepository.DeleteAsync(command.Id);
        }
        catch (Exception)
        {
            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.Salon));
        }

        return true;
    }
}
public class DeleteSalonCommandValidator
    : AbstractValidator<DeleteSalonCommand>
{
    public DeleteSalonCommandValidator()
    {

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SharedValidationMessages.Required);



    }
}
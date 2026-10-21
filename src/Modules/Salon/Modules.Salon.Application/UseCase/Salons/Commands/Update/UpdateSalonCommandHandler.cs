using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Validation.Resources;
using Mapster;
using Modules.Salon.Application.Contract.UseCase.Salons.Commands;
using Modules.Salon.Domain.Interface;
using Modules.Product.Application.Contract.Interface.Salons;
using SharedKernel.Constants;
using SharedKernel.Helper;

namespace Modules.Product.Application.UseCase.Salons.Commands.Update;

public class UpdateSalonCommandHandler(ISalonCommandRepository colorCommandRepository,ISalonQueryRepository colorQueryRepository)
: ICommandHandler<UpdateSalonCommand, bool>
{
    public async Task<Result<bool>> Handle(
        UpdateSalonCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var color = await colorQueryRepository.GetByIdAsync(command.request.Id, cancellationToken);
            if (color == null)
                return Result.Error(MessageHelper.Format(AppMessages.NotFound, AppEntity.Salon));


            command.request.Adapt(color);
            await colorCommandRepository.UpdateAsync(color,cancellationToken);

        }
        catch (Exception)
        {
            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.Salon));
        }

        return true;
    }
}
public class UpdateSalonCommandValidator
    : AbstractValidator<UpdateSalonCommand>
{
    public UpdateSalonCommandValidator()
    {
        RuleFor(x => x.request.Name)
            .NotEmpty()
            .WithMessage(SharedValidationMessages.Required)
            .MaximumLength(200)
            .WithMessage(SharedValidationMessages.MaxLength);

        RuleFor(x => x.request.Id)
            .NotEmpty()
            .WithMessage(SharedValidationMessages.Required);

    }
}
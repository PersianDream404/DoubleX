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

namespace Modules.Salon.Application.UseCase.Salons.Commands.Create;

public class CreateSalonCommandHandler(ISalonCommandRepository SalonCommandRepository, ISalonQueryRepository colorQueryRepository)
: ICommandHandler<CreateSalonCommand, bool>
{
    public async Task<Result<bool>> Handle(
        CreateSalonCommand command,
        CancellationToken cancellationToken)
    {
        try
        {

            var color = Personnel.Domain.Entities.Salon.Create(command.request.Name);
            await SalonCommandRepository.AddAsync(color,cancellationToken);

        }
        catch (Exception)
        {
            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.Salon));
        }

        return true;
    }
}


public class CreateSalonCommandValidator : AbstractValidator<CreateSalonCommand>
{
    public CreateSalonCommandValidator()
    {
        RuleFor(x => x.request)
                   .NotNull()
                   .WithMessage(SharedValidationMessages.Required);

        RuleFor(x => x.request.Name)
            .NotEmpty()
            .WithMessage(SharedValidationMessages.Required)
            .MaximumLength(200)
            .WithMessage(SharedValidationMessages.MaxLength);



    }
   
}
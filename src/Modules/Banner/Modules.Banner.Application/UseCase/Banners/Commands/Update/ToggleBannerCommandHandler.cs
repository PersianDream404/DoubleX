using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Validation.Resources;
using Modules.Banner.Application.Contract.UseCase.Banners.Commands;
using Modules.Banner.Domain.Interface;
using Modules.Product.Application.Contract.Interface.Banners;
using SharedKernel.Constants;
using SharedKernel.Helper;

namespace Modules.Product.Application.UseCase.Banners.Commands.Update;

public class ToggleBannerCommandHandler(IBannerCommandRepository colorCommandRepository,IBannerQueryRepository colorQueryRepository)
: ICommandHandler<ToggleBannerCommand, bool>
{
    public async Task<Result<bool>> Handle(
        ToggleBannerCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var color = await colorQueryRepository.GetByIdAsync(command.Id, cancellationToken);
            if (color == null)
                return Result.Error(MessageHelper.Format(AppMessages.NotFound, AppEntity.Banner));

         
            await colorCommandRepository.ToggleAsync(color,cancellationToken);

        }
        catch (Exception)
        {
            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.Banner));
        }

        return true;
    }
}
public class ToggleBannerCommandValidator
    : AbstractValidator<ToggleBannerCommand>
{
    public ToggleBannerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SharedValidationMessages.Required);

    }
}
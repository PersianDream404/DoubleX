using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Validation.Resources;
using Mapster;
using Modules.Banner.Application.Contract.UseCase.Banners.Commands;
using Modules.Banner.Domain.Interface;
using Modules.Product.Application.Contract.Interface.Banners;
using SharedKernel.Constants;
using SharedKernel.Helper;

namespace Modules.Product.Application.UseCase.Banners.Commands.Update;

public class UpdateBannerCommandHandler(IBannerCommandRepository colorCommandRepository,IBannerQueryRepository colorQueryRepository)
: ICommandHandler<UpdateBannerCommand, bool>
{
    public async Task<Result<bool>> Handle(
        UpdateBannerCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var color = await colorQueryRepository.GetByIdAsync(command.request.Id, cancellationToken);
            if (color == null)
                return Result.Error(MessageHelper.Format(AppMessages.NotFound, AppEntity.Banner));


            command.request.Adapt(color);
            await colorCommandRepository.UpdateAsync(color,cancellationToken);

        }
        catch (Exception)
        {
            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.Banner));
        }

        return true;
    }
}
public class UpdateBannerCommandValidator
    : AbstractValidator<UpdateBannerCommand>
{
    public UpdateBannerCommandValidator()
    {

        RuleFor(x => x.request.Id)
            .NotEmpty()
            .WithMessage(SharedValidationMessages.Required);

        //RuleFor(x => x.request.BannerCode)
        //    .NotEmpty()
        //    .WithMessage(SharedValidationMessages.Required)
        //    .MaximumLength(250)
        //    .WithMessage(SharedValidationMessages.MaxLength);

        //RuleFor(x => x.request.BannerName)
        //    .NotEmpty()
        //    .WithMessage(SharedValidationMessages.Required)
        //    .MaximumLength(250)
        //    .WithMessage(SharedValidationMessages.MaxLength);

    }
}
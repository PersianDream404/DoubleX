using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Validation.Resources;
using Modules.Product.Application.Contract.Interface.Banners;
using SharedKernel.Constants;
using SharedKernel.Helper;
using Modules.Banner.Domain.Interface;
using Modules.Banner.Application.Contract.UseCase.Banners.Commands;

namespace Modules.Banner.Application.UseCase.Banners.Commands.Delete;

public class DeleteBannerCommandHandler(IBannerCommandRepository colorCommandRepository,IBannerQueryRepository colorQueryRepository)
: ICommandHandler<DeleteBannerCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeleteBannerCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var color = await colorQueryRepository.GetByIdProjectedAsync(command.Id, cancellationToken);
            if (color == null)
                return Result.Error(MessageHelper.Format(AppMessages.NotFound, AppEntity.Banner));

            await colorCommandRepository.DeleteAsync(command.Id);
        }
        catch (Exception)
        {
            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.Banner));
        }

        return true;
    }
}
public class DeleteBannerCommandValidator
    : AbstractValidator<DeleteBannerCommand>
{
    public DeleteBannerCommandValidator()
    {

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SharedValidationMessages.Required);



    }
}
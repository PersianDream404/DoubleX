using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Validation.Resources;
using Mapster;
using Modules.Banner.Application.Contract.UseCase.Banners.Commands;
using Modules.Banner.Domain.Entities;
using Modules.Banner.Domain.Interface;
using Modules.Product.Application.Contract.Interface.Banners;
using SharedKernel.Constants;
using SharedKernel.Helper;

namespace Modules.Banner.Application.UseCase.Banners.Commands.Create;

public class CreateBannerCommandHandler(IBannerCommandRepository BannerCommandRepository, IBannerQueryRepository colorQueryRepository)
: ICommandHandler<CreateBannerCommand, bool>
{
    public async Task<Result<bool>> Handle(
        CreateBannerCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
     
            var color = command.request.Adapt<BannerEntity>();
            await BannerCommandRepository.AddAsync(color,cancellationToken);

        }
        catch (Exception)
        {
            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.Banner));
        }

        return true;
    }
}


public class CreateBannerCommandValidator : AbstractValidator<CreateBannerCommand>
{
    public CreateBannerCommandValidator()
    {
        RuleFor(x => x.request)
                   .NotNull()
                   .WithMessage(SharedValidationMessages.Required);

        RuleFor(x => x.request.Title)
            .NotEmpty()
            .WithMessage(SharedValidationMessages.Required)
            .MaximumLength(200)
            .WithMessage(SharedValidationMessages.MaxLength);

        RuleFor(x => x.request.Description)
            .MaximumLength(1000)
            .WithMessage(SharedValidationMessages.MaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.request.Description));

        RuleFor(x => x.request.ImageUrl)
            .MaximumLength(500)
            .WithMessage(SharedValidationMessages.MaxLength)
            //.Must(StringHelper.BeValidUrl)
            //.WithMessage(SharedValidationMessages.Invalid)
            .When(x => !string.IsNullOrWhiteSpace(x.request.ImageUrl));

        RuleFor(x => x.request.Url)
            .MaximumLength(500)
            .WithMessage(SharedValidationMessages.MaxLength)
            .Must(StringHelper.BeValidUrl)
            .WithMessage(SharedValidationMessages.Invalid)
            .When(x => !string.IsNullOrWhiteSpace(x.request.Url));

        RuleFor(x => x.request.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage(SharedValidationMessages.GreaterThanOrEqualToZero);

        RuleFor(x => x.request.Type)
            .IsInEnum()
            .WithMessage(SharedValidationMessages.Invalid);

        RuleFor(x => x.request.EndDate)
            .GreaterThanOrEqualTo(x => x.request.StartDate!.Value)
            .WithMessage("تاریخ پایان باید بزرگتر یا مساوی تاریخ شروع باشد.")
            .When(x => x.request.StartDate.HasValue && x.request.EndDate.HasValue);

    }
   
}
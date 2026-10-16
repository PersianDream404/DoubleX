using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.Validation.Resources;
using Modules.Banner.Application.Contract.DTOs.Banners.Get;
using Modules.Banner.Application.Contract.UseCase.Banners.Queries;
using Modules.Product.Application.Contract.Interface.Banners;
using SharedKernel.Constants;
using SharedKernel.Helper;

namespace Modules.Banner.Application.UseCase.Banners.Queries.Get;

public class GetByIdBannerQueryHandler(IBannerQueryRepository BannerQueryRepository)
: IQueryHandler<GetByIdBannerQuery, GetByIdBannerResponseDto>
{
    public async Task<Result<GetByIdBannerResponseDto>> Handle(
        GetByIdBannerQuery query,
        CancellationToken cancellationToken)
    {

        try
        {
            var color = await BannerQueryRepository.GetByIdProjectedAsync(query.Id, cancellationToken);
            if (color == null)
                return Result.Error(MessageHelper.Format(AppMessages.NotFound, AppEntity.Banner));
            return color;
        }
        catch (Exception)
        {
            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.Banner));
        }

    }
}
public class GetAllBannerQueryValidator
    : AbstractValidator<GetByIdBannerQuery>
{
    public GetAllBannerQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SharedValidationMessages.Required);
    }
}

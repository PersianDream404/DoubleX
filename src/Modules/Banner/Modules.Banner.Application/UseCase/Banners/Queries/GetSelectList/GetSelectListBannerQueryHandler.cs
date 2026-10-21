using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Banner.Application.Contract.DTOs.Banners.GetAll;
using Modules.Banner.Application.Contract.UseCase.Banners.Queries;
using Modules.Product.Application.Contract.Interface.Banners;
using SharedKernel.Constants;
using SharedKernel.Helper;

namespace Modules.Product.Application.UseCase.Banners.Queries.GetAll;

public class GetSelectListBannerQueryHandler(IBannerQueryRepository BannerQueryRepository)
: IQueryHandler<GetSelectListBannerQuery, PagedList<GetSelectListBannerResponseDto>>
{
    public async Task<Result<PagedList<GetSelectListBannerResponseDto>>> Handle(
        GetSelectListBannerQuery query,
        CancellationToken cancellationToken)
    {

        try
        {
            var result =await BannerQueryRepository.GetSelectListProjectedAsync(query.request, cancellationToken);

            return result;
        }
        catch (Exception)
        {
            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.Banner));
        }

    }
}
public class GetSelectListBannerQueryValidator
    : AbstractValidator<GetSelectListBannerQuery>
{
    public GetSelectListBannerQueryValidator()
    {
        //RuleFor(x => x.request.Q)
        //    .NotEmpty()
        //    .WithMessage("Banner Id is required");
    }
}

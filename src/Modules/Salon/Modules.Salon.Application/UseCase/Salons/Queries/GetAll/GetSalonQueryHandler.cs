using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Salon.Application.Contract.DTOs.Salons.GetAll;
using Modules.Salon.Application.Contract.UseCase.Salons.Queries;
using Modules.Product.Application.Contract.Interface.Salons;
using SharedKernel.Constants;
using SharedKernel.Helper;

namespace Modules.Product.Application.UseCase.Salons.Queries.GetAll;

public class GetSalonQueryHandler(ISalonQueryRepository SalonQueryRepository)
: IQueryHandler<GetAllSalonQuery, PagedList<GetAllSalonResponseDto>>
{
    public async Task<Result<PagedList<GetAllSalonResponseDto>>> Handle(
        GetAllSalonQuery query,
        CancellationToken cancellationToken)
    {

        try
        {
            var result =await SalonQueryRepository.GetAllProjectedAsync(query.request, cancellationToken);

            return result;
        }
        catch (Exception)
        {
            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.Salon));
        }

    }
}
public class GetAllSalonQueryValidator
    : AbstractValidator<GetAllSalonQuery>
{
    public GetAllSalonQueryValidator()
    {
        //RuleFor(x => x.request.Q)
        //    .NotEmpty()
        //    .WithMessage("Salon Id is required");
    }
}

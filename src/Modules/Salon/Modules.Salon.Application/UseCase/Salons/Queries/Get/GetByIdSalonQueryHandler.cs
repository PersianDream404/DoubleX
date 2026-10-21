using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.Validation.Resources;
using Modules.Salon.Application.Contract.DTOs.Salons.Get;
using Modules.Salon.Application.Contract.UseCase.Salons.Queries;
using Modules.Product.Application.Contract.Interface.Salons;
using SharedKernel.Constants;
using SharedKernel.Helper;

namespace Modules.Salon.Application.UseCase.Salons.Queries.Get;

public class GetByIdSalonQueryHandler(ISalonQueryRepository SalonQueryRepository)
: IQueryHandler<GetByIdSalonQuery, GetByIdSalonResponseDto>
{
    public async Task<Result<GetByIdSalonResponseDto>> Handle(
        GetByIdSalonQuery query,
        CancellationToken cancellationToken)
    {

        try
        {
            var color = await SalonQueryRepository.GetByIdProjectedAsync(query.Id, cancellationToken);
            if (color == null)
                return Result.Error(MessageHelper.Format(AppMessages.NotFound, AppEntity.Salon));
            return color;
        }
        catch (Exception)
        {
            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.Salon));
        }

    }
}
public class GetAllSalonQueryValidator
    : AbstractValidator<GetByIdSalonQuery>
{
    public GetAllSalonQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SharedValidationMessages.Required);
    }
}

using Ardalis.Result;
using Framwork.Bus.Query;
using Mapster;
using Modules.Personnel.Application.Common;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.Interfaces;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;
using Modules.Personnel.Domain.Interface;
using SharedKernel.Helper;

namespace Modules.Personnel.Application.UseCase.Personnel.Queries;

public class GetPersonnelByIdQueryHandler(
    IPersonnelQueryRepository PersonnelQueryRepository)
    : IQueryHandler<GetPersonnelByIdQuery, GetPersonnelDto>
{
    public async Task<Result<GetPersonnelDto>> Handle(GetPersonnelByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await PersonnelQueryRepository.GetByIdDtoAsync(query.Id, cancellationToken);
            if (entity is null)
                return Result.Error(Messages.PersonnelNotFound);

            return entity;
        }
        catch (Exception)
        {
            return Result.Error(Messages.ErrorInPersonnel);
        }
    }
}

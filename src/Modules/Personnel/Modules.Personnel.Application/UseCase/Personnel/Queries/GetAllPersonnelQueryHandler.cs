using Ardalis.Result;
using Framwork.Bus.Query;
using Framwork.PagedList;
using Mapster;
using Modules.Personnel.Application.Common;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.Interfaces;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;
using Modules.Personnel.Domain.Interface;
using SharedKernel.Helper;

namespace Modules.Personnel.Application.UseCase.Personnel.Queries;

public class GetAllPersonnelQueryHandler(
    IPersonnelQueryRepository PersonnelQueryRepository)
    : IQueryHandler<GetAllPersonnelQuery, PagedList<GetAllPersonalResponseDto>>
{
    public async Task<Result<PagedList<GetAllPersonalResponseDto>>> Handle(GetAllPersonnelQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await PersonnelQueryRepository.GetAllProjectedAsync(query.request, cancellationToken);
            return entity;
        }
        catch (Exception)
        {
            return Result.Error(Messages.ErrorInPersonnel);
        }
    }
}

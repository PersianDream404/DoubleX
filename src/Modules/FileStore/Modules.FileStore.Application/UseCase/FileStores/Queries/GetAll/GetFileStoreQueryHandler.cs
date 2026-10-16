using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.FileStore.Application.Contract.DTOs.FileStores.GetAll;
using Modules.FileStore.Application.Contract.Interface.FileStores;
using Modules.FileStore.Application.Contract.UseCase.FileStores.Queries;
using SharedKernel.Constants;
using SharedKernel.Helper;

namespace Modules.Product.Application.UseCase.FileStores.Queries.GetAll;

public class GetFileStoreQueryHandler(IFileStoreQueryRepository FileStoreQueryRepository)
: IQueryHandler<GetAllFileStoreQuery, PagedList<GetAllFileStoreResponseDto>>
{
    public async Task<Result<PagedList<GetAllFileStoreResponseDto>>> Handle(
        GetAllFileStoreQuery query,
        CancellationToken cancellationToken)
    {

        try
        {
            var result =await FileStoreQueryRepository.GetAllProjectedAsync(query.request, cancellationToken);

            return result;
        }
        catch (Exception)
        {
            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.FileStore));
        }

    }
}
public class GetAllFileStoreQueryValidator
    : AbstractValidator<GetAllFileStoreQuery>
{
    public GetAllFileStoreQueryValidator()
    {
        //RuleFor(x => x.request.Q)
        //    .NotEmpty()
        //    .WithMessage("FileStore Id is required");
    }
}

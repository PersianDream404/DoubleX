using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.FileStore.Application.Contract.DTOs.FileStores.Get;
using Modules.FileStore.Application.Contract.Interface.FileStores;
using Modules.FileStore.Application.Contract.UseCase.FileStores.Queries;
using SharedKernel.Constants;
using SharedKernel.Helper;

namespace Modules.Product.Application.UseCase.FileStores.Queries.Get;

public class GetByIdFileStoreQueryHandler(IFileStoreQueryRepository FileStoreQueryRepository)
: IQueryHandler<GetByIdFileStoreQuery, GetByIdFileStoreResponseDto>
{
    public async Task<Result<GetByIdFileStoreResponseDto>> Handle(
        GetByIdFileStoreQuery query,
        CancellationToken cancellationToken)
    {

        try
        {
            var fileStore = await FileStoreQueryRepository.GetByIdProjectedAsync(query.Id, cancellationToken);
            if (fileStore == null)
                return Result.Error(MessageHelper.Format(AppMessages.NotFound, AppEntity.FileStore));
            return fileStore;
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

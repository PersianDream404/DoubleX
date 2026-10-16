using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.FileStore.Application.Contract.Interface.FileStores;
using Modules.FileStore.Application.Contract.Interface.Services;
using Modules.FileStore.Application.Contract.UseCase.FileStores.Commands;
using Modules.Product.Domain.Interface.FileStores;
using SharedKernel.Constants;
using SharedKernel.Helper;
using SharedKernel.Interface.Repositories;

namespace Modules.FileStore.Application.UseCase.FileStores.Commands.Delete;

public class DeleteFileStoreCommandHandler(
    IFileStoreCommandRepository fileStoreCommandRepository,
    IFileStoreQueryRepository fileStoreQueryRepository,
    IFileStorageService fileStorageService,
    IUnitOfWork unitOfWork
    )
: ICommandHandler<DeleteFileStoreCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeleteFileStoreCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            var fileStore = await fileStoreQueryRepository.GetByIdAsync(command.Id, cancellationToken);
            if (fileStore == null || fileStore.FilePath is null)
                return Result.Error(MessageHelper.Format(AppMessages.NotFound, AppEntity.FileStore));
            await fileStoreCommandRepository.DeleteAsync(fileStore, cancellationToken);
            await fileStorageService.DeleteAsync(fileStore.FilePath, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.FileStore));
        }

        return true;
    }
}
public class DeleteFileStoreCommandValidator
    : AbstractValidator<DeleteFileStoreCommand>
{
    public DeleteFileStoreCommandValidator()
    {

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ارسال شناسه الزامی است.");



    }
}
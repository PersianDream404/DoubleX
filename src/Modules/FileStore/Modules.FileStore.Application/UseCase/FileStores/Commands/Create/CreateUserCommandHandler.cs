using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Mapster;
using Modules.FileStore.Application.Contract.DTOs.FileStores.Create;
using Modules.FileStore.Application.Contract.DTOs.FileUploader;
using Modules.FileStore.Application.Contract.Interface.FileStores;
using Modules.FileStore.Application.Contract.Interface.Services;
using Modules.FileStore.Application.Contract.UseCase.FileStores.Commands;
using Modules.FileStore.Application.Helper;
using Modules.Product.Domain.Interface.FileStores;
using SharedKernel.Constants;
using SharedKernel.Helper;
using SharedKernel.Interface.Repositories;

namespace Modules.FileStore.Application.UseCase.FileStores.Commands.Create;

public class CreateFileStoreCommandHandler(
    IFileStoreCommandRepository fileStoreCommandRepository,
    IFileStoreQueryRepository fileStoreQueryRepository,
    IFileStorageService fileStorageService,
    IUnitOfWork unitOfWork
    )
: ICommandHandler<CreateFileStoreCommand, CreateFileStoreResponseDto>
{
    public async Task<Result<CreateFileStoreResponseDto>> Handle(
        CreateFileStoreCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            var fileRequest = command.request.Adapt<UploadFileDto>();
            fileRequest.Content = command.request.Content;
            var fileResult = await fileStorageService.UploadAsync(fileRequest, cancellationToken);


            if (!await fileStoreQueryRepository.IsUniqueAsync(x => x.FilePath == fileResult.FilePath, true, cancellationToken))
                return Result.Error(MessageHelper.Format(AppMessages.Found, AppEntityFileStore.FilePath));
            var fileStore = fileResult.Adapt<Domain.Entities.FileStores.FileStore>();
            await fileStoreCommandRepository.AddAsync(fileStore, cancellationToken);

            await unitOfWork.CommitAsync(cancellationToken);
            return new CreateFileStoreResponseDto
            {
                Id = fileStore.Id,
                Path = fileStore.FilePath ?? "",
            };

        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.FileStore));
        }

    }
}


public class CreateFileStoreCommandValidator : AbstractValidator<CreateFileStoreCommand>
{
    public CreateFileStoreCommandValidator()
    {

        RuleFor(x => x.request.FileName)
       .NotEmpty();

        RuleFor(x => x.request.Length)
            .GreaterThan(0);

        RuleFor(x => x)
            .Must(HaveValidExtension)
            .WithMessage("فرمت فایل مجاز نیست.");

        RuleFor(x => x)
            .Must(HaveValidSize)
            .WithMessage("حجم فایل بیشتر از حد مجاز است.");

    }

    private static bool HaveValidExtension(CreateFileStoreCommand dto)
    {
        var extension = Path.GetExtension(dto.request.FileName);

        return FileStoreRules.IsValidExtension(
            dto.request.Category,
            extension);
    }

    private static bool HaveValidSize(CreateFileStoreCommand dto)
    {
        return FileStoreRules.IsValidSize(
            dto.request.Category,
            dto.request.Length);
    }
}
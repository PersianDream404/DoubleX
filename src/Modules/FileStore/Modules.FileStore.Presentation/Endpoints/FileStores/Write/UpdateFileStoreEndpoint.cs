using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.FileStore.Application.Contract.DTOs.FileStores.Create;
using Modules.FileStore.Application.Contract.DTOs.FileStores.Update;
using Modules.FileStore.Application.Contract.UseCase.FileStores.Commands;
using Modules.FileStore.Domain.Enums;
using SharedKernel.Constants;
using SharedKernel.Helper;
using SharedKernel.Interface;
using static Modules.FileStore.Presentation.Endpoints.FileStores.Write.CreateFileStoreEndpoint;

namespace Modules.FileStore.Presentation.Endpoints.FileStores.Write;



public static class UpdateFileStoreEndpoint
{
    private class UpdateFileStore
    {
        public long Id { get; set; }
        public IFormFile File { get; set; } = null!;
        public FileStoreCategory Category { get; set; }
    }
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut($"{ApiInfo.Prefix}/{{id}}", handler: async (

                  long id,

                    [FromForm] UpdateFileStore requestForm,
                  [FromServices] ICommandBus _commandBus, CancellationToken ct
                ) =>
            {

                if (id != requestForm.Id)
                    return BadRequest(AppMessages.BadRequest);

                UpdateFileStoreRequestDto request = new UpdateFileStoreRequestDto
                {
                    Id = id,
                    FileName = requestForm.File.FileName,
                    ContentType = requestForm.File.ContentType,
                    Length = requestForm.File.Length,
                    Content = requestForm.File.OpenReadStream(),
                    Category = requestForm.Category
                };

                var result = await _commandBus.Send<UpdateFileStoreCommand, CreateFileStoreResponseDto>
                                 (new UpdateFileStoreCommand(request), ct);

                if (!result.IsSuccess)
                {
                    var message = result.GetErrorMessage();
                    return BadRequest(message);
                }

                return Ok(result.Value,MessageHelper.Format(AppMessages.Edit, AppEntity.FileStore));


            })
                .DisableAntiforgery()
                .WithTags(ApiInfo.Tag);
        }
    }
}
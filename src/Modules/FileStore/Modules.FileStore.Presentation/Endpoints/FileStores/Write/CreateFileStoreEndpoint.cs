using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.FileStore.Application.Contract.DTOs.FileStores.Create;
using Modules.FileStore.Application.Contract.UseCase.FileStores.Commands;
using Modules.FileStore.Domain.Enums;
using SharedKernel.Constants;
using SharedKernel.Helper;
using SharedKernel.Interface;

namespace Modules.FileStore.Presentation.Endpoints.FileStores.Write;



public static class CreateFileStoreEndpoint
{
    private class CreateFileStore
    {
        public IFormFile File { get; set; } = null!;
        public FileStoreCategory Category { get; set; }
    }
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost($"{ApiInfo.Prefix}", handler: async (


                    [FromForm] CreateFileStore requestForm,

                  [FromServices] ICommandBus _commandBus, CancellationToken ct
                ) =>
            {
                CreateFileStoreRequestDto request = new CreateFileStoreRequestDto
                {
                    FileName = requestForm.File.FileName,
                    ContentType = requestForm.File.ContentType,
                    Length = requestForm.File.Length,
                    Content = requestForm.File.OpenReadStream(),
                    Category = requestForm.Category
                };
                var result = await _commandBus.Send<CreateFileStoreCommand, CreateFileStoreResponseDto>
                                 (new CreateFileStoreCommand(request), ct);

                if (!result.IsSuccess)
                {
                    var message = result.GetErrorMessage();
                    return BadRequest(message);
                }

                return Ok(result.Value, MessageHelper.Format(AppMessages.Create, AppEntity.FileStore));


            })
                .DisableAntiforgery()
                .WithTags(ApiInfo.Tag);
        }
    }
}
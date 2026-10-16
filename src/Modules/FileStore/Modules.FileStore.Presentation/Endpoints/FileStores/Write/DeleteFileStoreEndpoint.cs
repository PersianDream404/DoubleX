using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.FileStore.Application.Contract.UseCase.FileStores.Commands;
using SharedKernel.Constants;
using SharedKernel.Helper;
using SharedKernel.Interface;

namespace Modules.FileStore.Presentation.Endpoints.FileStores.Write;



public static class DeleteFileStoreEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete($"{ApiInfo.Prefix}/{{id}}", handler: async (

                  int id,
                   
                  [FromServices] ICommandBus _commandBus, CancellationToken ct
                ) =>
            {


                var result = await _commandBus.Send<DeleteFileStoreCommand, bool>
                                 (new DeleteFileStoreCommand(id), ct);

                if (!result.IsSuccess)
                {
                    var message = result.GetErrorMessage();
                    return BadRequest(message);
                }

                return Ok(MessageHelper.Format(AppMessages.Delete, AppEntity.FileStore));


            })
                .WithTags(ApiInfo.Tag);
        }
    }
}
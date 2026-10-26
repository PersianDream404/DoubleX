using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.UseCase.ServiceCategories.Commands.Delete;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServiceCategories.Delete;

public sealed class DeleteServiceCategoryEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapDelete($"{ServiceApiInfo.Prefix}/Categories/{{id:long}}", async (
                    long id,
                    [FromQuery] long salonId,
                    [FromServices] ICommandBus bus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bus.Send<DeleteServiceCategoryCommand, bool>(
                        new DeleteServiceCategoryCommand(salonId, id), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "دسته‌بندی حذف شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(ServiceApiInfo.Tag);
    }
}

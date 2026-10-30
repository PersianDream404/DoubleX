using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.UseCase.ServiceCategories.Commands.Deactivate;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServiceCategories.Deactivate;

public sealed class DeactivateServiceCategoryEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPatch($"{ServiceApiInfo.Prefix}/Categories/{{id:long}}/Deactivate", async (
                    long id,
                    [FromServices] ICommandBus bus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();

                    var result = await bus.Send<DeactivateServiceCategoryCommand, bool>(
                        new DeactivateServiceCategoryCommand(salonId, id), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "دسته‌بندی غیرفعال شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ServiceApiInfo.Tag);
    }
}

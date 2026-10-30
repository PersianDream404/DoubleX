using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServiceCategories;
using Modules.Service.Application.Contract.UseCase.ServiceCategories.Commands.Update;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServiceCategories.Update;

public sealed class UpdateServiceCategoryEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPut($"{ServiceApiInfo.Prefix}/Categories/{{id:long}}", async (
                    long id,
                    [FromBody] UpdateServiceCategoryRequestDto request,
                    [FromServices] ICommandBus bus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();

                    var result = await bus.Send<UpdateServiceCategoryCommand, bool>(
                        new UpdateServiceCategoryCommand(id, request with { SalonId = salonId }), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "دسته‌بندی خدمت به‌روزرسانی شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ServiceApiInfo.Tag);
    }
}

using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.UseCase.Services.Commands.Update;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.Services.Update;

public sealed class UpdateServiceEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPut($"{ServiceApiInfo.Prefix}/{{id:long}}", async (
                    long id,
                    [FromBody] UpdateServiceRequestDto request,
                    [FromServices] ICommandBus bus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();

                    var result = await bus.Send<UpdateServiceCommand, bool>(
                        new UpdateServiceCommand(id, request with { SalonId = salonId }), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "خدمت به‌روزرسانی شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ServiceApiInfo.Tag);
    }
}

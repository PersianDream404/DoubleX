using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.Delete;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServiceTariffs.Delete;

public sealed class DeleteServiceTariffEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapDelete($"{ServiceApiInfo.Prefix}/Tariffs/{{id:long}}", async (
                    long id,
                [FromServices] ICommandBus bus,
                HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                if (!TryGetSalonId(context, out var salonId))
                    return Unauthorized();

                var result = await bus.Send<DeleteServiceTariffCommand, bool>(
                    new DeleteServiceTariffCommand(id, salonId), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "تعرفه و خدمات ثبت‌شده در آن حذف شدند." })
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ServiceApiInfo.Tag);
    }
}

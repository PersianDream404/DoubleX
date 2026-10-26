using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Reception.Application.Contract.DTOs.ReceptionServiceMaterials.Register;
using Modules.Reception.Application.Contract.UseCase.ReceptionServiceMaterials.Commands.Register;
using SharedKernel.Interface;

namespace Modules.Reception.Presentation.Endpoints.ReceptionServiceMaterials.Write;

public sealed class RegisterReceptionServiceMaterialEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPost(
                $"{ReceptionApiInfo.Prefix}/{{receptionId:long}}/services/{{serviceId:long}}/materials",
                async (
                    long receptionId,
                    long serviceId,
                    [FromBody] RegisterReceptionServiceMaterialRequestDto request,
                    [FromServices] ICommandBus commandBus,
                    HttpContext httpContext,
                    CancellationToken cancellationToken) =>
                {
                    if (!ReceptionApiInfo.TryGetSalonId(httpContext, out var salonId))
                    {
                        return BadRequest("شناسهٔ سالن از هویت کاربر دریافت نشد.");
                    }

                    var result = await commandBus.Send<RegisterReceptionServiceMaterialCommand, bool>(
                        new RegisterReceptionServiceMaterialCommand(
                            receptionId,
                            serviceId,
                            request with { SalonId = salonId }),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "مصرف واقعی ماده ثبت شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ReceptionApiInfo.Tag);
    }
}

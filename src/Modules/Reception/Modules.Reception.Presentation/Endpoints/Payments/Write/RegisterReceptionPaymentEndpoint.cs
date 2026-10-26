using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Reception.Application.Contract.DTOs.Payments.Register;
using Modules.Reception.Application.Contract.UseCase.Payments.Commands.Register;
using SharedKernel.Interface;

namespace Modules.Reception.Presentation.Endpoints.Payments.Write;

public sealed class RegisterReceptionPaymentEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPost($"{ReceptionApiInfo.Prefix}/{{receptionId:long}}/payments", async (
                    long receptionId,
                    [FromBody] RegisterReceptionPaymentRequestDto request,
                    [FromServices] ICommandBus commandBus,
                    HttpContext httpContext,
                    CancellationToken cancellationToken) =>
                {
                    if (!ReceptionApiInfo.TryGetSalonId(httpContext, out var salonId))
                    {
                        return BadRequest("شناسهٔ سالن از هویت کاربر دریافت نشد.");
                    }

                    var result = await commandBus.Send<RegisterReceptionPaymentCommand, bool>(
                        new RegisterReceptionPaymentCommand(
                            receptionId,
                            request with { SalonId = salonId }),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "پرداخت پذیرش ثبت شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ReceptionApiInfo.Tag);
    }
}

using Framwork.Bus.Command;
using Framwork.Extensions;
using Identity.Application.Contract.DTOs.Users;
using Identity.Application.Contract.Users.Commands;
using Identity.Domain.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Interface;

namespace Identity.Presentation.Endpoints.Admin.Write;

public sealed class CreateSalonWithAdminEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPost("/api/Identity/admin/salons", async (
                    [FromBody] CreateSalonWithAdminRequestDto request,
                    [FromServices] ICommandBus commandBus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await commandBus.Send<
                        CreateSalonWithAdminCommand,
                        CreateSalonWithAdminResponseDto>(
                        new CreateSalonWithAdminCommand(request),
                        cancellationToken);

                    if (!result.IsSuccess)
                    {
                        return BadRequest(result.Errors.ToArray());
                    }

                    return Ok(
                        result.Value,
                        "سالن و حساب مدیر آن با نقش مدیر سالن ایجاد شد.");
                })
                .RequireAuthorization(policy => policy.RequireRole(Roles.SuperAdmin))
                .WithTags("System administration");
    }
}

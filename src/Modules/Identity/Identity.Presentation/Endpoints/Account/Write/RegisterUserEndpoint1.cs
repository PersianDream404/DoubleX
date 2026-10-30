using Framwork.Bus.Command;
using Framwork.Extensions;
using Identity.Application.Contract.DTOs.Users;
using Identity.Application.Contract.Users.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ParsizCRM.API.Features.Account;
using SharedKernel.Interface;

namespace Identity.Presentation.Endpoints.Account.Write;

public static class RegisterUserEndpoint1
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost($"{ApiInfo.Prefix}/testToken", handler: async (


                  [FromBody] RegisterUserRequestDto request,
                  [FromServices] ICommandBus _commandBus,
                  [FromServices] IJwtService jwtService
                ) =>
            {

                return Ok("tes");


            })
                .RequireAuthorization()
                .WithTags(ApiInfo.Tag);
        }
    }
}

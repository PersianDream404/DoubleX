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

namespace Identity.Presentation.Users.Create;



public static class RegisterUserEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost($"{ApiInfo.Prefix}/Register", handler: async (


                  [FromBody] RegisterUserRequestDto request,
                  [FromServices] ICommandBus _commandBus,
                  [FromServices] IJwtService jwtService
                ) =>
            {

                var result = await _commandBus.Send<RegisterUserCommand, RegisterUserResponseDto>(
                new RegisterUserCommand(request));

                if (!result.IsSuccess)
                {
                    var message = result.GetErrorMessage();
                    return BadRequest(message); 
                }
                var tokenResult = jwtService.CreateToken(result.Value.Id, result.Value.Roles);
                return Ok(new
                {
                    tokenResult= tokenResult,
                    info= result.Value
                });


            })
                .WithTags(ApiInfo.Tag);
        }
    }
}
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

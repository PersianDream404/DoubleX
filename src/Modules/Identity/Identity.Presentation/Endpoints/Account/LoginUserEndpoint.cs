using Framwork.Bus.Query;
using Framwork.Extensions;
using Identity.Application.Contract.DTOs.Authentications;
using Identity.Application.Contract.DTOs.Users;
using Identity.Application.Contract.Users.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using ParsizCRM.API.Features.Account;
using SharedKernel.Interface;

namespace Identity.Presentation.Users.Create;

public static class LoginUserEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost($"{ApiInfo.Prefix}/Login", handler: async (


                  [FromBody] LoginRequestDto request,
                  [FromServices] IQueryBus queryBus,
                  [FromServices] ILogger<EndPoint> logger,
                  [FromServices] IJwtService jwtService
                ) =>
            {
                logger.LogInformation("start Login");
                var result = await queryBus.Send<LoginUserQuery, RegisterUserResponseDto>(
                new LoginUserQuery(request));

                if (!result.IsSuccess)
                {
                    var message = result.GetErrorMessage();
                    return BadRequest(message);
                }
                var tokenResult = jwtService.CreateToken(result.Value.Id, result.Value.Roles);
                return Ok(new
                {
                    tokenResult = tokenResult,
                    info = result.Value
                });


            })
                .WithTags(ApiInfo.Tag);
        }
    }
}
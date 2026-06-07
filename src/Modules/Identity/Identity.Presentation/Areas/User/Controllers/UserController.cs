
using Framwork.Bus.Query;
using Framwork.Extensions;
using Identity.Application.Contract.DTOs.Authentications;
using Identity.Application.Contract.DTOs.Users;
using Identity.Application.Contract.Services;
using Identity.Application.Contract.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace Identity.Presentation.Areas.User.Controllers;

[Area("User")]
//[Authorize]
public class UserController(IAuthenticationService authenticationService) : Controller
{

    //[Route("/test")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {

        var result = await authenticationService.RegisterUser(new RegisterUserViewModel
        {

        }, ct);
        return View("تست");
    }


}
//[Area("User")]
//[Authorize]
public class User1Controller(IAuthenticationService authenticationService, IQueryBus _queryBus) : Controller
{

    //[Route("/test")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {

        var result = await _queryBus.Send<GetAllUserQuery, GetAllUserResponseDto>(
             new GetAllUserQuery(new GetAllUserRequestDto {Q="sdsdd"}));

        if (!result.IsSuccess)
        {
            var message = result.GetErrorMessage();
        }
        return View("تست");
    }


}
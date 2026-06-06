
using Identity.Application.Contract.Services;
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

        var result = await authenticationService.RegisterUser(new Application.Contract.DTOs.RegisterUserViewModel
        {

        }, ct);
        return Content("تست");
    }


}

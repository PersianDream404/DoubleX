using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.Extensions;
using Identity.Application.Contract.DTOs.Users;
using Identity.Application.Contract.Users.Queries;
using Identity.Domain.Entities;
using Identity.Domain.Interface;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Events;
using SharedKernel.Events.Logs;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace Identity.Application.Users.Queries.GetAll;

public class LoginUserQueryHandler(
    IUserQueryRepository userQueryRepository,
        IEventBus eventBus,
    UserManager<ApplicationUser> _userManager)
: IQueryHandler<LoginUserQuery, RegisterUserResponseDto>
{
    public async Task<Result<RegisterUserResponseDto>> Handle(
        LoginUserQuery query,
        CancellationToken cancellationToken)
    {

        // اصلاح کد ناقص بلاک Catch جهت انتشار صحیح رویداد شکست پرداخت
        await eventBus.PublishAsync(
            new LogEventRequest("لاگ تستی"), cancellationToken);


        var phoneNumber = query.request.UserName.NormalizePhoneNumber();

        var user = await userQueryRepository.GetByMobile(phoneNumber);

        if (user == null)
        return    Result.Unauthorized("Invalid phone number or password." );

        var validPassword = await _userManager.CheckPasswordAsync(user, query.request.Password);

        if (!validPassword)
            return Result.Unauthorized( "Invalid phone number or password." );


        var roles = await _userManager.GetRolesAsync(user);
        return new RegisterUserResponseDto
        {
            Id = user.Id,
            Roles = roles.ToList(),

        };
    }
}
public class LoginUserQueryValidator
    : AbstractValidator<LoginUserQuery>
{
    public LoginUserQueryValidator()
    {
        RuleFor(x => x.request.UserName)
            .NotEmpty()
            .WithMessage("User Id is required");
        RuleFor(x => x.request.Password)
            .NotEmpty()
            .WithMessage("User Id is required");
    }
}
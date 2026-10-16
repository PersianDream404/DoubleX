using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Extensions;
using Identity.Application.Contract.DTOs.Users;
using Identity.Application.Contract.Users.Queries;
using Identity.Domain.Entities;
using Identity.Domain.Interface;
using Mapster;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Identity.Application.Users.Queries.Create;

public class RegisterUserCommandHandler(IUserCommandRepository userCommandRepository
    , IUserQueryRepository userQueryRepository,
    UserManager<ApplicationUser> _userManager,
    RoleManager<IdentityRole<long>> _roleManager
    )
: ICommandHandler<RegisterUserCommand, RegisterUserResponseDto>
{

    public async Task<Result<RegisterUserResponseDto>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var normalizedPhoneNumber = command.request.Mobile.NormalizePhoneNumber();

        var userExists = await userQueryRepository.GetByMobile(normalizedPhoneNumber);

        if (userExists != null)
            return Result.Error("User already exists with this phone number.");

        var user = new ApplicationUser
        {
            PhoneNumber = normalizedPhoneNumber,
            UserName = normalizedPhoneNumber,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, command.request.Password);

        if (!result.Succeeded)
        {
            //return BadRequest(new
            //{
            //    message = "User creation failed.",
            //    errors = result.Errors.Select(e => e.Description)
            //});

            return Result.Error("خطا در ثبت کاربر رخ داده است");
        }

        var roles = await _userManager.GetRolesAsync(user);
        return new RegisterUserResponseDto
        {
            Id = user.Id,
            Roles = roles.ToList(),

        };
    }
}
public class RegisterUserCommandValidator
    : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        //RuleFor(x => x.request.FirstName)
        //    .NotEmpty()
        //    .WithMessage("FirstName is required");

        //RuleFor(x => x.request.LastName)
        //    .NotEmpty()
        //    .WithMessage("LastName is required");

        RuleFor(x => x.request.Mobile)
            .NotEmpty()
            .WithMessage("Mobile is required")
            .MinimumLength(5)
            .WithMessage("Mobile MinLengh 5")
            ;

        RuleFor(x => x.request.Password)
            .NotEmpty()
            .WithMessage("Password is required")
                        .MinimumLength(3)
            .WithMessage("Password MinLengh 3")
            ;
    }
}
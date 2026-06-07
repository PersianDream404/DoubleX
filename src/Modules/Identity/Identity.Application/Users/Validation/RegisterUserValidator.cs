using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Users.Validation;

using FluentValidation;
using Identity.Application.Contract.DTOs.Authentications;

public class RegisterUserValidator : AbstractValidator<RegisterUserViewModel>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

    }
}

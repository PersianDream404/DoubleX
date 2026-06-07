using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Identity.Application.Contract.DTOs.Users;
using Identity.Application.Contract.Users.Queries;
using Identity.Domain.Entities;
using Identity.Domain.Interface;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Users.Queries.Create;

public class CreateUserCommandHandler(IUserCommandRepository userCommandRepository)
: ICommandHandler<CreateUserCommand, bool>
{
    public async Task<Result<bool>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = command.request.Adapt<User>();
        await userCommandRepository.AddAsync(user);

        return true;
    }
}
public class CreateUserCommandValidator
    : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.request.FirstName)
            .NotEmpty()
            .WithMessage("FirstName is required");

        RuleFor(x => x.request.LastName)
            .NotEmpty()
            .WithMessage("LastName is required");

        RuleFor(x => x.request.UserName)
            .NotEmpty()
            .WithMessage("UserName is required")
            .MinimumLength(5)
            .WithMessage("UserName MinLengh 5")
            ;
    }
}
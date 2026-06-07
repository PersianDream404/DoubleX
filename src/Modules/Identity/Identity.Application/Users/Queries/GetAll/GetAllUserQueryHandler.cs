using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Identity.Application.Contract.DTOs.Users;
using Identity.Application.Contract.Users.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Users.Queries.GetAll;

public class GetUserQueryHandler
: IQueryHandler<GetAllUserQuery, GetAllUserResponseDto>
{
    public async Task<Result<GetAllUserResponseDto>> Handle(
        GetAllUserQuery query,
        CancellationToken cancellationToken)
    {
        var user = new GetAllUserResponseDto
        {
            Id =1,
            FirstName = "Ali",
            LastName="shabnai"
        };

        return Result.Error("");
    }
}
public class GetAllUserQueryValidator
    : AbstractValidator<GetAllUserQuery>
{
    public GetAllUserQueryValidator()
    {
        RuleFor(x => x.request.Q)
            .NotEmpty()
            .WithMessage("User Id is required");
    }
}
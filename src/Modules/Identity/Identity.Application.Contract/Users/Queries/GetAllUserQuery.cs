using Framwork.Bus.Query;
using Identity.Application.Contract.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Contract.Users.Queries;

public record GetAllUserQuery(GetAllUserRequestDto request) : IQuery<GetAllUserResponseDto>;

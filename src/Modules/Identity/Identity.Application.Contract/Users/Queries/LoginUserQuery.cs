using Framwork.Bus.Query;
using Identity.Application.Contract.DTOs.Authentications;
using Identity.Application.Contract.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Contract.Users.Queries;

public record LoginUserQuery(LoginRequestDto request) : IQuery<RegisterUserResponseDto>;

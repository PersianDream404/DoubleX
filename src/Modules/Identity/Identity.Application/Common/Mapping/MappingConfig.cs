using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Common.Mapping;

using Identity.Application.Contract.DTOs.Authentications;
using Identity.Application.Contract.DTOs.Users;
using Identity.Application.Contract.Users.Queries;
using Identity.Domain.Entities;
using Mapster;
using SharedKernel.Constants;

public static class MappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<RegisterUserViewModel, User>.NewConfig();
        TypeAdapterConfig<CreateUserRequestDto, User>.NewConfig()
             .Map(dest => dest.Password, src => HashingHelper.HashPassword(src.Password??AppSetting.DefultPassword,AppSetting.Salt))
            ;

    }
}

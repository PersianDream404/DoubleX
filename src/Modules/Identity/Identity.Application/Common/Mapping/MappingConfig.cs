using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Common.Mapping;

using Identity.Application.Contract.DTOs.Authentications;
using Identity.Domain.Entities;
using Mapster;

public static class MappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<RegisterUserViewModel, User>.NewConfig();

    }
}

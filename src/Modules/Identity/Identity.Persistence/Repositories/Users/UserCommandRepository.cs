using Identity.Domain.Entities;
using Identity.Domain.Interface;
using Identity.Persistence.Context;
using Infrastructure.Repositories;
using SharedKernel.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Persistence.Repositories.Users;


public class UserCommandRepository
    : CommandRepository<User>, IUserCommandRepository
{
    public UserCommandRepository(WriteDbContext context) : base(context)
    {
    }

 
}

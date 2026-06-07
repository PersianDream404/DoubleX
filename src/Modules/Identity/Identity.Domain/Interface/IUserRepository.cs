using Identity.Domain.Entities;
using SharedKernel.Interface;
using SharedKernel.Interface.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Interface;

public interface IUserQueryRepository : IQueryRepository<User>
{
}


public interface IUserCommandRepository : ICommandRepository<User>
{
}

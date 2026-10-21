using Identity.Domain.Entities;
using SharedKernel.Interface;
using SharedKernel.Interface.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Interface;

public interface IUserQueryRepository 
{
    Task<ApplicationUser> GetByMobile(string mobile);
}


public interface IUserCommandRepository
{
}

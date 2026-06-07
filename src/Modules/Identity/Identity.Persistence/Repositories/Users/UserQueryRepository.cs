using Identity.Domain.Entities;
using Identity.Domain.Interface;
using Identity.Persistence.Context;
using Infrastructure.Repositories;
using SharedKernel.Interface;

namespace Identity.Persistence.Repositories.Users;

public class UserQueryRepository
    : QueryRepository<User>, IUserQueryRepository
{
    public UserQueryRepository(ReadDbContext context) : base(context)
    {
    }
}

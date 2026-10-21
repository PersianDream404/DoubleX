using Identity.Domain.Entities;
using Identity.Domain.Interface;
using Identity.Persistence.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Interface;

namespace Identity.Persistence.Repositories.Users;

public class UserQueryRepository
    : IUserQueryRepository
{
    private readonly IdentityWriteDbContext _context;
    public UserQueryRepository(IdentityWriteDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser?> GetByMobile(string mobile)
    {
       return await _context.Users
             .FirstOrDefaultAsync(x => x.PhoneNumber == mobile);
    }
}

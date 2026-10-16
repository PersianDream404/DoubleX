//using Ardalis.Result;
//using Identity.Application.Contract.DTOs.Authentications;
//using SharedKernel.Interface;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Identity.Application.Contract.Services;

//public interface IAuthenticationService:IScopedDependency
//{

//    Task<Result<LoginResultViewModel>> LoginUser(LoginViewModel request, CancellationToken ct = default);

//    //Task<List<DropdownItem>> GetUserEmployeesSelectListAsync(CancellationToken ct = default);

//    Task<Result> RegisterUser(RegisterUserViewModel viewModel, CancellationToken ct = default);
//    Task<Result> ChangePassword(ChangePasswordViewModel request, CancellationToken ct = default);
//}
using Identity.Application.Contract.DTOs.Authentications;
using SharedKernel.Enums;
using SharedKernel.Interface;

public interface IJwtService 
{
    JwtTokenResponseDto CreateToken(long Id, List<string> roles);
    string GetClaim(string token, string claimType);
    (List<string> roles, long id) ExteractToken(string token);
}

using Ardalis.Result;
using Identity.Application.Contract.DTOs;
using SharedKernel.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Contract.Services;

public interface IAuthenticationService:IScopedDependency
{

    Task<Result<LoginResultViewModel>> LoginUser(LoginViewModel request, CancellationToken ct = default);
  
    //Task<List<DropdownItem>> GetUserEmployeesSelectListAsync(CancellationToken ct = default);

    Task<Result> RegisterUser(RegisterUserViewModel viewModel, CancellationToken ct = default);
    Task<Result> ChangePassword(ChangePasswordViewModel request, CancellationToken ct = default);
}

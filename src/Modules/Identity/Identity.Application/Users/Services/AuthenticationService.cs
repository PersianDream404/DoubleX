using Ardalis.Result;
using FluentValidation;
using Identity.Application.Contract.DTOs.Authentications;
using Identity.Application.Contract.Services;
using Identity.Domain.Entities;
using Mapster;
using ParsizCRM.API.Shared.Constants;
using SharedKernel.Helper;
using SharedKernel.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Users.Services;

public class AuthenticationService(
    IRepository<User> _repository,
     IValidator<RegisterUserViewModel> _validator
    ) : IAuthenticationService, IScopedDependency
{
    public Task<Result> ChangePassword(ChangePasswordViewModel request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<LoginResultViewModel>> LoginUser(LoginViewModel request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> RegisterUser(RegisterUserViewModel viewModel, CancellationToken ct = default)
    {
        try
        {
            var result = await _validator.ValidateAsync(viewModel, ct);

            if (!result.IsValid)
            {
                var errors = result.Errors
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return Result.Error(errors.First());
            }

            var user = viewModel.Adapt<User>();
            //user.Password=Hash

            await _repository.AddAsync(user);

            return Result.Success();
        }
        catch (Exception)
        {

            return Result.Error(MessageHelper.Format(AppMessages.ErrorIn, AppEntity.User));
        }
    }
}

using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Extensions;
using Identity.Application.Contract.DTOs.Users;
using Identity.Application.Contract.Users.Commands;
using Identity.Domain.Entities;
using Identity.Domain.Security;
using Microsoft.AspNetCore.Identity;
using Modules.Salon.Application.Contract.DTOs.Salons.Create;
using Modules.Salon.Application.Contract.UseCase.Salons.Commands;

namespace Identity.Application.Users.Commands.CreateSalonWithAdmin;

public sealed class CreateSalonWithAdminCommandHandler(
    ICommandBus commandBus,
    UserManager<ApplicationUser> userManager)
    : ICommandHandler<CreateSalonWithAdminCommand, CreateSalonWithAdminResponseDto>
{
    public async Task<Result<CreateSalonWithAdminResponseDto>> Handle(
        CreateSalonWithAdminCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var mobile = request.AdminMobile.NormalizePhoneNumber();

        if (await userManager.FindByNameAsync(mobile) is not null)
        {
            return Result.Invalid(new ValidationError(
                nameof(request.AdminMobile),
                "با این شماره همراه قبلاً حسابی ثبت شده است."));
        }

        var salonResult = await commandBus.Send<CreateSalonCommand, long>(
            new CreateSalonCommand(new CreateSalonRequestDto { Name = request.SalonName }),
            cancellationToken);
        if (!salonResult.IsSuccess)
        {
            return Result.Error(string.Join("; ", salonResult.Errors));
        }

        var salonId = salonResult.Value;
        var user = ApplicationUser.Register(mobile, request.AdminFullName, salonId);
        var createUserResult = await userManager.CreateAsync(user, request.AdminPassword);
        if (!createUserResult.Succeeded)
        {
            var errors = string.Join("; ", createUserResult.Errors.Select(error => error.Description));
            var salonRemoved = await RemoveSalonAsync(salonId);
            return Result.Error(salonRemoved
                ? $"ساخت حساب مدیر سالن انجام نشد: {errors}"
                : $"ساخت حساب مدیر سالن انجام نشد: {errors} سالن با شناسه {salonId} باقی مانده و نیاز به بررسی دارد.");
        }

        var roleResult = await userManager.AddToRoleAsync(user, Roles.SalonAdmin);
        if (!roleResult.Succeeded)
        {
            var userRemoved = await userManager.DeleteAsync(user);
            if (!userRemoved.Succeeded)
            {
                var errors = string.Join("; ", userRemoved.Errors.Select(error => error.Description));
                return Result.Error(
                    $"تخصیص نقش مدیر سالن انجام نشد و حذف حساب ساخته‌شده نیز ناموفق بود. " +
                    $"شناسه سالن: {salonId}، شناسه حساب: {user.Id}. {errors}");
            }

            var salonRemoved = await RemoveSalonAsync(salonId);
            var roleErrors = string.Join("; ", roleResult.Errors.Select(error => error.Description));
            return Result.Error(salonRemoved
                ? $"تخصیص نقش مدیر سالن انجام نشد: {roleErrors}"
                : $"تخصیص نقش مدیر سالن انجام نشد: {roleErrors} سالن با شناسه {salonId} باقی مانده و نیاز به بررسی دارد.");
        }

        return Result.Success(new CreateSalonWithAdminResponseDto(
            salonId,
            user.Id,
            Roles.SalonAdmin));
    }

    private async Task<bool> RemoveSalonAsync(long salonId)
    {
        var result = await commandBus.Send<DeleteSalonCommand, bool>(
            new DeleteSalonCommand(salonId));
        return result.IsSuccess;
    }
}

public sealed class CreateSalonWithAdminCommandValidator
    : AbstractValidator<CreateSalonWithAdminCommand>
{
    public CreateSalonWithAdminCommandValidator()
    {
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.SalonName)
                .NotEmpty()
                .MaximumLength(200);
            RuleFor(command => command.Request.AdminFullName)
                .NotEmpty()
                .MaximumLength(200);
            RuleFor(command => command.Request.AdminMobile)
                .NotEmpty()
                .MinimumLength(5);
            RuleFor(command => command.Request.AdminPassword)
                .NotEmpty()
                .MinimumLength(3);
        });
    }
}

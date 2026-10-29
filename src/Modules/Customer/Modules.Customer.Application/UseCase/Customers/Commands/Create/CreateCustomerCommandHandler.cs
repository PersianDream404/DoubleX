using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Validation.Resources;
using Modules.Customer.Application.Contract.DTOs.Customers.Create;
using Modules.Customer.Application.Contract.Interfaces.Customers;
using Modules.Customer.Application.Contract.UseCase.Customers.Commands.Create;
using Modules.Customer.Domain.Interface;
using CustomerEntity = Modules.Customer.Domain.Entities.Customer;

namespace Modules.Customer.Application.UseCase.Customers.Commands.Create;

public sealed class CreateCustomerCommandHandler(
    ICustomerCommandRepository commandRepository,
    ICustomerQueryRepository queryRepository)
    : ICommandHandler<CreateCustomerCommand, bool>
{
    public async Task<Result<bool>> Handle(
        CreateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        if (!await queryRepository.SalonExistsAsync(request.SalonId, cancellationToken))
        {
            return Result.NotFound("سالن موردنظر پیدا نشد.");
        }

        if (await queryRepository.MobilePhoneExistsAsync(
                request.SalonId,
                request.MobilePhone.Trim(),
                null,
                cancellationToken))
        {
            return Result.Conflict("مشتری‌ای با این شماره موبایل در سالن وجود دارد.");
        }

        var customer = CustomerEntity.Create(
            request.FullName.Trim(),
            request.BirthDate,
            request.NationalCode.Trim(),
            request.Address.Trim(),
            request.AllergyDescription?.Trim(),
            request.JobTitle?.Trim(),
            request.LandlinePhone?.Trim(),
            request.MobilePhone.Trim(),
            request.SalonId);

        await commandRepository.AddAsync(customer, cancellationToken);
        return true;
    }
}

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(command => command.Request)
            .NotNull()
            .WithMessage(SharedValidationMessages.Required);

        RuleFor(command => command.Request.FullName)
            .NotEmpty()
            .WithMessage(SharedValidationMessages.Required)
            .MaximumLength(250)
            .WithMessage(SharedValidationMessages.MaxLength);
        RuleFor(command => command.Request.NationalCode)
            .NotEmpty()
            .WithMessage(SharedValidationMessages.Required)
            .Length(10)
            .WithMessage("کد ملی باید دقیقاً ۱۰ رقم باشد.")
            .Matches("^[0-9]{10}$")
            .WithMessage("کد ملی باید فقط شامل ۱۰ رقم باشد.");
        RuleFor(command => command.Request.Address)
            .NotEmpty()
            .WithMessage(SharedValidationMessages.Required)
            .MaximumLength(1000)
            .WithMessage(SharedValidationMessages.MaxLength);
        RuleFor(command => command.Request.AllergyDescription)
            .MaximumLength(1000)
            .WithMessage(SharedValidationMessages.MaxLength);
        RuleFor(command => command.Request.JobTitle)
            .MaximumLength(150)
            .WithMessage(SharedValidationMessages.MaxLength);
        RuleFor(command => command.Request.LandlinePhone)
            .MaximumLength(20)
            .WithMessage(SharedValidationMessages.MaxLength);
        RuleFor(command => command.Request.MobilePhone)
            .NotEmpty()
            .WithMessage(SharedValidationMessages.Required)
            .MaximumLength(20)
            .WithMessage(SharedValidationMessages.MaxLength);
        RuleFor(command => command.Request.SalonId)
            .GreaterThan(0)
            .WithMessage(SharedValidationMessages.GreaterThanZero);
        RuleFor(command => command.Request.BirthDate)
            .Must(date => date < DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("تاریخ تولد باید قبل از امروز باشد.");
    }
}

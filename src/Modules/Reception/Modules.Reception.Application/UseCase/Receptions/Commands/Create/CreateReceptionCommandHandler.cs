using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Modules.CashRegister.Application.Contract.DTOs.Sessions;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Queries;
using Modules.CashRegister.Domain.Entities;
using Modules.Customer.Application.Contract.DTOs.Customers.Get;
using Modules.Customer.Application.Contract.UseCase.Customers.Queries.Get;
using Modules.Appointment.Application.Contract.DTOs.Appointments;
using Modules.Appointment.Application.Contract.UseCase.Appointments.Queries.GetById;
using Modules.Appointment.Domain.Entities;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;
using Modules.Inventory.Application.Contract.DTOs;
using Modules.Inventory.Application.Contract.DTOs.Materials;
using Modules.Inventory.Application.Contract.UseCase;
using Modules.Inventory.Domain.Entities;
using Modules.Reception.Application.Contract.Interfaces;
using Modules.Reception.Application.Contract.UseCase.Receptions.Commands.Create;
using Modules.Reception.Domain.Entities;
using Modules.Reception.Application.UseCase.ReceptionServiceMaterials.Commands.Register;
using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;
using Modules.Service.Application.Contract.UseCase.Services.Queries.GetById;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Queries.GetById;
using ReceptionEntity = Modules.Reception.Domain.Entities.Reception;
using Modules.Reception.Application.Contract.DTOs.Receptions.Create;
using Framwork.Extensions;

namespace Modules.Reception.Application.UseCase.Receptions.Commands.Create;

public sealed class CreateReceptionCommandHandler(
    IReceptionCommandRepository repository,
    IQueryBus queryBus,
    ICommandBus commandBus)
    : ICommandHandler<CreateReceptionCommand, long>
{
    public async Task<Result<long>> Handle(
        CreateReceptionCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var tariffResult = await queryBus.Send<GetServiceTariffByIdQuery, ServiceTariffDto>(
            new GetServiceTariffByIdQuery(request.ServiceTariffId),
            cancellationToken);
        if (!tariffResult.IsSuccess ||
            tariffResult.Value.SalonId != request.SalonId ||
            tariffResult.Value.ValidFrom > DateTimeOffset.UtcNow ||
            (tariffResult.Value.ValidTo.HasValue &&
             tariffResult.Value.ValidTo.Value < DateTimeOffset.UtcNow))
        {
            return Result.Invalid(new ValidationError("تعرفهٔ معتبر و متعلق به سالن انتخاب‌شده پیدا نشد."));
        }

        var customerResult = await queryBus.Send<GetCustomerByIdQuery, CustomerDetailsDto>(
            new GetCustomerByIdQuery(request.CustomerId),
            cancellationToken);
        if (!customerResult.IsSuccess ||
            !customerResult.Value.Status ||
            customerResult.Value.SalonId != request.SalonId)
        {
            return Result.Invalid(new ValidationError("مشتری فعال و متعلق به سالن انتخاب‌شده نیست."));
        }

        if (request.AppointmentId.HasValue)
        {
            if (await repository.AppointmentAlreadyLinkedAsync(
                    request.AppointmentId.Value,
                    cancellationToken))
            {
                return Result.Conflict("این نوبت قبلاً به پذیرش متصل شده است.");
            }

            var appointmentResult = await queryBus.Send<
                GetAppointmentByIdQuery,
                AppointmentDetailsDto>(
                new GetAppointmentByIdQuery(request.AppointmentId.Value, request.SalonId),
                cancellationToken);
            if (!appointmentResult.IsSuccess ||
                appointmentResult.Value.Status != AppointmentStatus.Confirmed ||
                appointmentResult.Value.CustomerId != request.CustomerId ||
                appointmentResult.Value.SalonId != request.SalonId ||
                !tariffResult.Value.Services.Any(service =>
                    service.ServiceId == appointmentResult.Value.ServiceId))
            {
                return Result.Invalid(new ValidationError(
                    "نوبت تأییدشده و متعلق به مشتری و سالن انتخاب‌شده پیدا نشد."));
            }
        }

        var receptionistResult = await queryBus.Send<GetPersonnelByIdQuery, GetByIdPersonnelResponseDto>(
            new GetPersonnelByIdQuery(request.ReceptionistId),
            cancellationToken);
        if (!receptionistResult.IsSuccess ||
            !receptionistResult.Value.Status ||
            receptionistResult.Value.SalonId != request.SalonId)
        {
            return Result.Invalid(new ValidationError("پذیرشگر فعال و متعلق به سالن انتخاب‌شده نیست."));
        }

        var reception = ReceptionEntity.Create(
            request.SalonId,
            request.CustomerId,
            customerResult.Value.FullName,
            request.ReceptionistId,
            request.ServiceTariffId,
            request.ReceptionDate,
            request.Notes,
            request.AppointmentId);

        var serviceMaterials = new List<(CreateReceptionServiceMaterialDto Request, ReceptionService Service)>();
        foreach (var serviceRequest in request.Services)
        {
            if (serviceRequest.ServiceTariffId != request.ServiceTariffId)
            {
                return Result.Invalid(new ValidationError(
                    "تعرفهٔ همه خدمات باید با تعرفهٔ انتخاب‌شده برای پذیرش یکسان باشد."));
            }

            var tariffItem = tariffResult.Value.Services
                .SingleOrDefault(item => item.ServiceId == serviceRequest.ServiceId);
            if (tariffItem is null)
            {
                return Result.Invalid(new ValidationError("خدمت در تعرفهٔ انتخاب‌شده تعریف نشده است."));
            }

            var serviceResult = await queryBus.Send<GetServiceByIdQuery, ServiceDto>(
                new GetServiceByIdQuery(serviceRequest.ServiceId),
                cancellationToken);
            if (!serviceResult.IsSuccess ||
                !serviceResult.Value.IsActive ||
                serviceResult.Value.SalonId != request.SalonId)
            {
                return Result.Invalid(new ValidationError("خدمت فعال و متعلق به سالن انتخاب‌شده نیست."));
            }

            if (serviceRequest.PersonnelId.HasValue)
            {
                var personnelResult = await queryBus.Send<
                    GetPersonnelByIdQuery,
                    GetByIdPersonnelResponseDto>(
                    new GetPersonnelByIdQuery(serviceRequest.PersonnelId.Value),
                    cancellationToken);
                if (!personnelResult.IsSuccess ||
                    !personnelResult.Value.Status ||
                    personnelResult.Value.SalonId != request.SalonId)
                {
                    return Result.Invalid(new ValidationError("پرسنل فعال و متعلق به سالن انتخاب‌شده نیست."));
                }
            }

            if (!reception.TryAddService(
                    serviceRequest.ServiceId,
                    serviceRequest.ServiceTariffId,
                    serviceResult.Value.Name,
                    serviceRequest.PersonnelId,
                    serviceRequest.Quantity,
                    tariffItem.Amount,
                    serviceRequest.DiscountAmount))
            {
                return Result.Invalid(new ValidationError("اطلاعات یکی از خدمات پذیرش معتبر نیست."));
            }

            var addedService = reception.Services.Last();
            foreach (var materialRequest in serviceRequest.Materials)
            {
                var materialResult = await queryBus.Send<GetMaterialByIdQuery, MaterialDto>(
                    new GetMaterialByIdQuery(materialRequest.MaterialId),
                    cancellationToken);
                if (!materialResult.IsSuccess || !materialResult.Value.IsActive)
                {
                    return Result.Invalid(new ValidationError("مادهٔ فعال موردنظر پیدا نشد."));
                }

                serviceMaterials.Add((materialRequest, addedService));
            }
        }

        return await repository.AddAsync(
            reception,
            async (createdReception, transactionCancellationToken) =>
        {
            var pendingMaterials = new List<(
                CreateReceptionServiceMaterialDto Request,
                ReceptionService Service,
                string IdempotencyKey)>();
            foreach (var (materialRequest, service) in serviceMaterials)
            {
                var idempotencyKey =
                    RegisterReceptionServiceMaterialCommandHandler.CreateInventoryIdempotencyKey(
                        createdReception.Id,
                        service.Id,
                        materialRequest.IdempotencyKey);
                if (!service.TryAddMaterial(
                        materialRequest.MaterialId,
                        materialRequest.Quantity,
                        idempotencyKey,
                        materialRequest.Description))
                {
                    return Result.Error("ثبت مادهٔ مصرفی برای این خدمت در وضعیت فعلی مجاز نیست.");
                }

                pendingMaterials.Add((materialRequest, service, idempotencyKey));
            }

            if (pendingMaterials.Count > 0 &&
                !await repository.SaveChangesAsync(transactionCancellationToken))
            {
                return Result.Conflict("پذیرش هم‌زمان تغییر کرده است.");
            }

            foreach (var (materialRequest, service, idempotencyKey) in pendingMaterials)
            {
                var receptionMaterial = service.Materials.SingleOrDefault(
                    item => item.IdempotencyKey == idempotencyKey);
                if (receptionMaterial is null)
                {
                    return Result.Error("رکورد مادهٔ مصرفی در پذیرش پیدا نشد.");
                }

                var movementResult = await commandBus.Send<
                    RegisterStockMovementCommand,
                    RegisterStockMovementResponseDto>(
                    new RegisterStockMovementCommand(new RegisterStockMovementRequestDto
                    {
                        MaterialId = materialRequest.MaterialId,
                        Type = StockMovementType.Consumption,
                        Quantity = materialRequest.Quantity,
                        OccurredAt = DateTimeOffset.UtcNow,
                        ReferenceType = "ReceptionServiceMaterial",
                        ReferenceId = receptionMaterial.Id,
                        Description = materialRequest.Description,
                        IdempotencyKey = idempotencyKey
                    }, createdReception.MaximumAdditionalMaterialCharge),
                    transactionCancellationToken);
                if (!movementResult.IsSuccess)
                {
                    return Result.Error(movementResult.GetErrorMessage());
                }

                if (movementResult.Value.UnitCost is null || movementResult.Value.TotalCost is null)
                {
                    return Result.Error("بهای FIFO مادهٔ مصرفی در دسترس نیست.");
                }

                if (!createdReception.MarkMaterialMovementRegistered(
                        service.Id,
                        idempotencyKey,
                        movementResult.Value.UnitCost.Value,
                        movementResult.Value.TotalCost.Value))
                {
                    return Result.Error("مبلغ FIFO ماده از محدودهٔ مجاز پذیرش بیشتر است.");
                }
            }

            foreach (var payment in request.Payments)
            {
                var isCash = string.Equals(
                    payment.PaymentMethod.Trim(),
                    "نقدی",
                    StringComparison.OrdinalIgnoreCase);
                long? sessionId = null;
                var acceptedAt = DateTimeOffset.UtcNow;
                if (isCash)
                {
                    var activeSession = await queryBus.Send<
                        GetCashRegisterSessionQuery,
                        CashRegisterSessionDto>(
                        new GetCashRegisterSessionQuery(payment.CashRegisterId!.Value, request.SalonId),
                        transactionCancellationToken);
                    if (!activeSession.IsSuccess ||
                        activeSession.Value is null ||
                        activeSession.Value.CashRegisterId != payment.CashRegisterId.Value ||
                        activeSession.Value.Status != CashRegisterSessionStatus.Open)
                    {
                        return Result.Invalid(new ValidationError(
                            "صندوق انتخاب‌شده جلسهٔ باز و فعال ندارد."));
                    }

                    sessionId = activeSession.Value.Id;
                }

                if (!createdReception.TryAddPayment(
                        payment.Amount,
                        payment.PaymentMethod,
                        payment.PaidAt,
                        payment.TransactionReference,
                        payment.IdempotencyKey,
                        payment.Description,
                        payment.CashRegisterId))
                {
                    return Result.Invalid(new ValidationError(
                        "پرداخت نامعتبر یا بیشتر از ماندهٔ پذیرش است."));
                }

                if (sessionId.HasValue)
                {
                    repository.AddCashRegisterPaymentOutbox(CashRegisterPaymentOutbox.Create(
                        request.SalonId,
                        payment.CashRegisterId!.Value,
                        sessionId.Value,
                        createdReception.Id,
                        command.UserId,
                        payment.Amount,
                        acceptedAt,
                        CashRegisterPaymentOutbox.CreateIdempotencyKey(
                            createdReception.Id,
                            payment.IdempotencyKey),
                        payment.Description));
                }
            }

            return Result.Success(createdReception.Id);
        },
            cancellationToken);
    }
}

public sealed class CreateReceptionCommandValidator : AbstractValidator<CreateReceptionCommand>
{
    public CreateReceptionCommandValidator()
    {
        RuleFor(command => command.UserId).GreaterThan(0);
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.SalonId).GreaterThan(0);
            RuleFor(command => command.Request.CustomerId).GreaterThan(0);
            RuleFor(command => command.Request.ReceptionistId).GreaterThan(0);
            RuleFor(command => command.Request.ServiceTariffId).GreaterThan(0);
            RuleFor(command => command.Request.ReceptionDate).NotEmpty();
            RuleFor(command => command.Request.Notes).MaximumLength(2000);
            RuleFor(command => command.Request.AppointmentId)
                .GreaterThan(0)
                .When(command => command.Request.AppointmentId.HasValue);
            RuleFor(command => command.Request.Services).NotNull().NotEmpty();
            RuleForEach(command => command.Request.Services).ChildRules(service =>
            {
                service.RuleFor(item => item.ServiceId).GreaterThan(0);
                service.RuleFor(item => item.ServiceTariffId).GreaterThan(0);
                service.RuleFor(item => item.PersonnelId)
                    .GreaterThan(0)
                    .When(item => item.PersonnelId.HasValue);
                service.RuleFor(item => item.Quantity)
                    .GreaterThan(0)
                    .LessThanOrEqualTo(1000000000m);
                service.RuleFor(item => item.DiscountAmount).GreaterThanOrEqualTo(0);
                service.RuleFor(item => item.Materials).NotNull();
                service.RuleForEach(item => item.Materials)
                    .ChildRules(material =>
                    {
                        material.RuleFor(item => item.MaterialId).GreaterThan(0);
                        material.RuleFor(item => item.Quantity)
                            .GreaterThan(0)
                            .LessThanOrEqualTo(999999999999999.999m)
                            .Must(value => decimal.Round(value, 3) == value);
                        material.RuleFor(item => item.IdempotencyKey).NotEmpty().MaximumLength(100);
                        material.RuleFor(item => item.Description).MaximumLength(1000);
                    });
            });
            RuleFor(command => command.Request.Payments).NotNull();
            RuleForEach(command => command.Request.Payments).ChildRules(payment =>
            {
                payment.RuleFor(item => item.Amount)
                    .GreaterThan(0)
                    .LessThanOrEqualTo(9999999999999999.99m)
                    .Must(value => decimal.Round(value, 2) == value);
                payment.RuleFor(item => item.PaymentMethod).NotEmpty().MaximumLength(50);
                payment.RuleFor(item => item.PaidAt).NotEmpty();
                payment.RuleFor(item => item.TransactionReference).MaximumLength(150);
                payment.RuleFor(item => item.IdempotencyKey).NotEmpty().MaximumLength(100);
                payment.RuleFor(item => item.Description).MaximumLength(1000);
                payment.RuleFor(item => item.CashRegisterId)
                    .GreaterThan(0)
                    .When(item => string.Equals(
                        item.PaymentMethod?.Trim(),
                        "نقدی",
                        StringComparison.OrdinalIgnoreCase));
                payment.RuleFor(item => item.CashRegisterId)
                    .Null()
                    .When(item => !string.Equals(
                        item.PaymentMethod?.Trim(),
                        "نقدی",
                        StringComparison.OrdinalIgnoreCase));
            });
        });
    }
}

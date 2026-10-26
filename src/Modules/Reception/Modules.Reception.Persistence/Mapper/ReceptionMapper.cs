using System.Linq.Expressions;
using Modules.Reception.Domain.Entities;
using Modules.Reception.Domain.ReadModels;
using ReceptionEntity = Modules.Reception.Domain.Entities.Reception;

namespace Modules.Reception.Persistence.Mapper;

public static class ReceptionMapper
{
    public static Expression<Func<ReceptionEntity, ReceptionListReadModel>> ToListReadModel() =>
        reception => new ReceptionListReadModel(
            reception.Id,
            reception.SalonId,
            reception.ReceptionNumber,
            reception.CustomerId,
            reception.CustomerName,
            reception.ReceptionistId,
            reception.ReceptionDate,
            reception.ReceptionStatus,
            reception.TotalAmount,
            reception.Payments
                .Where(payment => payment.PaymentStatus == ReceptionPaymentStatus.Succeeded)
                .Sum(payment => (decimal?)payment.Amount) ?? 0m,
            reception.TotalAmount - (reception.Payments
                .Where(payment => payment.PaymentStatus == ReceptionPaymentStatus.Succeeded)
                .Sum(payment => (decimal?)payment.Amount) ?? 0m),
            reception.ReceptionFinancialStatus);

    public static Expression<Func<ReceptionEntity, ReceptionDetailsReadModel>> ToDetailsReadModel() =>
        reception => new ReceptionDetailsReadModel(
            reception.Id,
            reception.SalonId,
            reception.ReceptionNumber,
            reception.CustomerId,
            reception.CustomerName,
            reception.ReceptionistId,
            reception.ReceptionDate,
            reception.ReceptionStatus,
            reception.SubTotal,
            reception.TotalDiscountAmount,
            reception.TotalAmount,
            reception.Payments
                .Where(payment => payment.PaymentStatus == ReceptionPaymentStatus.Succeeded)
                .Sum(payment => (decimal?)payment.Amount) ?? 0m,
            reception.TotalAmount - (reception.Payments
                .Where(payment => payment.PaymentStatus == ReceptionPaymentStatus.Succeeded)
                .Sum(payment => (decimal?)payment.Amount) ?? 0m),
            reception.ReceptionFinancialStatus,
            reception.Notes,
            reception.Services.Select(service => new ReceptionServiceReadModel(
                service.Id,
                service.ServiceId,
                service.ServiceTariffId,
                service.PersonnelId,
                service.ServiceName,
                service.Quantity,
                service.UnitPrice,
                service.DiscountAmount,
                service.FinalAmount,
                service.ServiceStatus,
                service.Materials.Select(material => new ReceptionServiceMaterialReadModel(
                    material.Id,
                    material.MaterialId,
                    material.Quantity,
                    material.UnitCost,
                    material.Description,
                    material.MovementRegistered,
                    material.MovementReversed)).ToList())).ToList(),
            reception.Payments.Select(payment => new ReceptionPaymentReadModel(
                payment.Id,
                payment.Amount,
                payment.PaymentMethod,
                payment.PaidAt,
                payment.PaymentStatus,
                payment.TransactionReference,
                payment.Description)).ToList());
}

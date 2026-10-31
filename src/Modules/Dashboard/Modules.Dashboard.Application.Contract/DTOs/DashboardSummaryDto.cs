namespace Modules.Dashboard.Application.Contract.DTOs;

public sealed record DashboardSummaryDto(
    int ReceptionCount,
    int CompletedReceptionCount,
    int InProgressReceptionCount,
    int CancelledReceptionCount,
    int AppointmentReceptionCount,
    int WalkInReceptionCount,
    int UniqueCustomerCount,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount,
    decimal PaidAmount,
    decimal OutstandingAmount,
    decimal RecordedExpenses,
    decimal CancelledExpenses);

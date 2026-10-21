namespace Shared.Contract.DTOs.Payments;

public sealed class CreatePaymentRequestDto
{
    public decimal Amount { get; init; }

    public long TrackingNumber { get; init; }

    public string ReturnUrl { get; init; } = default!;
}
public record VerifyPaymentRequestDto(
    int TransactionId
);

public class ParbadPaymentResultDto
{
    public string GatewayUrl { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

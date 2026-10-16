using Ardalis.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Shared.Contract.DTOs.Payments;

//public class PaymentService(
//    IHttpContextAccessor httpContextAccessor,
//    IOnlinePayment onlinePayment,
//    IPaymentCallbackUrlFactory callbackUrlFactory,
//    IUserTransactionService userTransactionService,
//    IMemoryCache cache
//) : IPaymentService
//{

//    #region Create Payment

//    public async Task<Result<ParbadPaymentResultDto>> CreatePaymentAsync(
//        CreatePaymentRequestDto request,
//        CancellationToken ct)
//    {
//        if (request.Amount <= 0)
//            return Result.Invalid(new ValidationError
//            {
//                Identifier = "Payment.Amount.Invalid",
//                ErrorMessage = "Amount must be greater than zero."
//            });

//        var transactionResult = await userTransactionService.CreateAsync(
//            new CreateUserTransactionRequestDto(
//                request.UserId,
//                request.NationalCode,
//                request.Amount,
//                TransactionType.Debit,
//                request.Reason,
//                request.Description,
//                request.CreatorId),
//            ct);

//        if (!transactionResult.IsSuccess)
//            return Result.Error(transactionResult.Errors);

//        var transaction = transactionResult.Value;

//        request.CallbackRoute.RouteValues["transactionId"] = transaction.Id;

//        var callbackUrl = callbackUrlFactory.CreateUrl(request.CallbackRoute);

//        var paymentResult = await onlinePayment.RequestAsync(invoice =>
//        {
//            invoice
//                .UseZarinPal()
//                .SetZarinPalData("پرداخت", "support@site.com")
//                .SetAmount(request.Amount)
//                .SetCallbackUrl(callbackUrl);

//            invoice.SetTrackingNumber(transaction.Id);
//        });

//        if (!paymentResult.IsSucceed)
//            return Result.Error("Failed to create payment link.");

//        return Result.Success(new ParbadPaymentResultDto
//        {
//            GatewayUrl = paymentResult.GatewayTransporter.Descriptor.Url,
//            Message = "Payment link created successfully."
//        });
//    }

//    #endregion

//    #region Verify Payment

//    public async Task<Result> VerifyPaymentAsync(
//        VerifyPaymentRequestDto request,
//        CancellationToken ct)
//    {
//        IPaymentFetchResult invoice;

//        try
//        {
//            invoice = await onlinePayment.FetchAsync();
//        }
//        catch
//        {
//            return Result.Error("Bank response invalid.");
//        }

//        if (invoice.Status != PaymentFetchResultStatus.ReadyForVerifying)
//            return Result.Error("Payment not ready for verification.");

//        if (!invoice.IsSucceed)
//        {
//            await userTransactionService.UpdateStatusAsync(
//                new UpdateUserTransactionRequestDto(
//                    request.TransactionId,
//                    PaymentStatus.Failed,
//                    null),
//                ct);

//            return Result.Error("Payment failed.");
//        }

//        var verifyResult = await onlinePayment.VerifyAsync(invoice);

//        if (!verifyResult.IsSucceed)
//        {
//            await userTransactionService.UpdateStatusAsync(
//                new UpdateUserTransactionRequestDto(
//                    request.TransactionId,
//                    PaymentStatus.Failed,
//                    verifyResult.TransactionCode),
//                ct);

//            return Result.Error("Bank verification failed.");
//        }

//        var markResult = await userTransactionService.MarkAsPaidAsync(
//            request.TransactionId,
//            verifyResult.TransactionCode!,
//            ct);

//        if (!markResult.IsSuccess)
//            return Result.Error(markResult.Errors);

//        return Result.Success();
//    }

//    #endregion

//    #region Helpers

//    private string GetReturnUrlCacheKey(int transactionId)
//        => $"payment:returnUrl:{transactionId}";

//    private string CreateCallbackUrl(
//        string controller,
//        string action,
//        IDictionary<string, string> queryParams)
//    {
//        var httpContext = httpContextAccessor.HttpContext;

//        if (httpContext is null)
//            throw new InvalidOperationException("HttpContext not available.");

//        var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

//        var queryString = string.Join("&",
//            queryParams.Select(q =>
//                $"{q.Key}={Uri.EscapeDataString(q.Value)}"));

//        return $"{baseUrl}/{controller}/{action}?{queryString}";
//    }

//    #endregion
//}

//public interface IPaymentService : IService
//{
//    Task<Result<ParbadPaymentResultDto>> CreatePaymentAsync(
//        CreatePaymentRequestDto request,
//        CancellationToken ct);

//    Task<Result> VerifyPaymentAsync(
//        VerifyPaymentRequestDto request,
//        CancellationToken ct);
//}

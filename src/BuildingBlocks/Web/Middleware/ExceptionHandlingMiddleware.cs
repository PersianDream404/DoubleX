using Framwork.Eceptions;
using SharedKernel.Models;
using System.Net;

namespace Web.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // Client disconnected or request was cancelled.
            return;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Unhandled exception processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            if (context.Response.HasStarted)
                throw;

            var (statusCode, message) = MapException(exception);

            context.Response.Clear();
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsJsonAsync(
                new ApiResponse(
                    message,
                    statusCode,
                    [message],
                    null),
                context.RequestAborted);
        }
    }

    private static (HttpStatusCode StatusCode, string Message) MapException(
        Exception exception)
    {
        return exception switch
        {
            BusinessRuleException =>
                (HttpStatusCode.Conflict, exception.Message),

            ArgumentNullException =>
                (HttpStatusCode.BadRequest, "پارامتر ارسالی نمی‌تواند خالی باشد."),

            ArgumentException =>
                (HttpStatusCode.BadRequest, "اطلاعات ارسالی نامعتبر است."),

            InvalidOperationException =>
                (HttpStatusCode.Conflict, "امکان انجام این عملیات در وضعیت فعلی وجود ندارد."),

            KeyNotFoundException =>
                (HttpStatusCode.NotFound, "اطلاعات موردنظر یافت نشد."),

            UnauthorizedAccessException =>
                (HttpStatusCode.Forbidden, "شما اجازه انجام این عملیات را ندارید."),

            NotImplementedException =>
                (HttpStatusCode.NotImplemented, "این قابلیت هنوز پیاده‌سازی نشده است."),

            TimeoutException =>
                (HttpStatusCode.RequestTimeout, "زمان انتظار برای انجام درخواست به پایان رسید."),

            _ =>
                (HttpStatusCode.InternalServerError, "خطای داخلی سرور رخ داده است.")
        };
    }
}